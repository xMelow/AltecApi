using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Altec.Api.Record.Printers;

namespace Altec.Api.Domain.Printers;

public class PrinterDiscovery
{
    private const int PrinterPort = 9100;
    
    public async Task<IReadOnlyList<Printer>> Discover(List<string> subnets)
    {
        List<Printer> printers = new List<Printer>();
        foreach (var subnet in subnets)
        {
            var ipAddresses = GetSubnetIpAddresses(subnet);
            var foundIps = await ScanForOpenPorts(ipAddresses);
            var foundPrinters = await GetPrinterDetails(foundIps);
            printers.AddRange(foundPrinters);
        }
        return printers;
    }

    private IReadOnlyList<IPAddress> GetSubnetIpAddresses(string subnet)
    {
        var subnetData = ParseSubnet(subnet);
        var bytes = subnetData.baseIp.GetAddressBytes();
        var startIp = (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
        var numberOfAddresses = (int)Math.Pow(2, 32 - subnetData.prefixLength);
        List<IPAddress> ipAddresses = new List<IPAddress>();
        for (int i = startIp; i < numberOfAddresses + startIp; i++)
        {
            var ipBytes = new byte[]
            {
                (byte)(i >> 24),
                (byte)(i >> 16),
                (byte)(i >> 8),
                (byte)i
            };
            var ipAddress = new IPAddress(ipBytes);
            if (IsValidHostAddress(ipAddress))
                ipAddresses.Add(ipAddress);
        }
        return ipAddresses;
    }

    private async Task<IEnumerable<IPAddress>> ScanForOpenPorts(IReadOnlyList<IPAddress> ipAddresses)
    {
        var tasks = ipAddresses.Select(ip => IsPortOpen(ip, PrinterPort, 200));
        var results = await Task.WhenAll(tasks);
        var foundIps = ipAddresses
            .Zip(results, (ip, isOpen) => (ip, isOpen))
            .Where(x => x.isOpen)
            .Select(x => x.ip);
        return foundIps;
    }

    private async Task<List<Printer>> GetPrinterDetails(IEnumerable<IPAddress> foundIps)
    {
        var printerTask = foundIps.Select(async ip =>
            {
                try
                {
                    var printerInfo = await GetPrinterInfo(ip);
                    var shortDnsName = printerInfo.printerDnsName.Split(".")[0];
                    return new Printer(printerInfo.printerDnsName, shortDnsName, ip.ToString(), printerInfo.printerModelName, PrinterPort);
                }
                catch
                {
                    return null;
                }
            });
        var foundPrinters = await Task.WhenAll(printerTask);
        return foundPrinters.Where(p => p != null && p.PrinterModel != "Unknown").ToList();
    }

    private (IPAddress baseIp, int prefixLength) ParseSubnet(string subnet)
    {
        var parts = subnet.Split("/");
        return (IPAddress.Parse(parts[0]), Convert.ToInt32(parts[1]));
    }

    private bool IsValidHostAddress(IPAddress ip)
    {
        var bytes = ip.GetAddressBytes();
        return bytes[3] != 0 && bytes[3] != 255;
    }

    private async Task<bool> IsPortOpen(IPAddress ip, int port, int timeoutMs)
    {
        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(ip, port);
            var timeoutTask = Task.Delay(timeoutMs);
            await Task.WhenAny(connectTask, timeoutTask);
            return client.Connected;
        }
        catch
        {
            return false;
        }
    }

    private async Task<(string printerDnsName, string printerModelName)> GetPrinterInfo(IPAddress ip)
    {
        var dnsTask = GetPrinterDnsName(ip);
        var modelTask = GetPrinterModelName(ip);
        
        await Task.WhenAll(dnsTask, modelTask);
        return (await dnsTask, await modelTask);
    }

    private async Task<string> GetPrinterDnsName(IPAddress ip)
    {
        try
        {
            var dnsTask = Dns.GetHostEntryAsync(ip);
            var timeoutTask = Task.Delay(100);
            var completed = await Task.WhenAny(dnsTask, timeoutTask);
            if (completed == dnsTask)
                return (await dnsTask).HostName;
            
            return "Not found";
        }
        catch
        {
            return "Not found";
        }
    }

    private async Task<string> GetPrinterModelName(IPAddress ip)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(ip, PrinterPort);
        var stream = client.GetStream();
        byte[] command = Encoding.ASCII.GetBytes("~!T\r\n");
        await stream.WriteAsync(command, 0, command.Length);
        
        await Task.Delay(200);
        
        byte[] buffer = new byte[1024];
        var readTask = stream.ReadAsync(buffer, 0, buffer.Length);
        var timeoutTask = Task.Delay(400);
        var completed = await Task.WhenAny(readTask, timeoutTask);
    
        if (completed == readTask)
        {
            var bytesRead = await readTask;
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }
        return "Unknown";
    }

    public async Task<PrinterInfo> GetPrinterSettings(IPAddress ip)
    {
        var program = string.Join("\r\n",
            "OUT \"SPEED=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"SPEED\")",
            "OUT \"DENSITY=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"DENSITY\")",
            "OUT \"SIZE=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"SIZE\")",
            "OUT \"GAP=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"GAP\")",
            "OUT \"DIRECTION=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"DIRECTION\")",
            "OUT \"CODEPAGE=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"CODEPAGE\")",
            "OUT \"SENSOR=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"SENSOR\")",
            "OUT \"RIBBON=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"RIBBON\")",
            "OUT \"RIBBONSENSOR=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"RIBBONSENSOR\")",
            "OUT \"COUNTRY=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"COUNTRY\")",
            "OUT \"SHIFT=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"SHIFT\")",
            "OUT \"REFERENCE=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"REFERENCE\")",
            "OUT \"POSTACTION=\";GETSETTING$(\"CONFIG\",\"TSPL\",\"POSTACTION\")"
        );
        var response = await SendPrinterCommand(ip, program);
        
        return ParseSettings(response);
    }

    private PrinterInfo ParseSettings(string response)
    {
        var settings = response
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => l.Contains('='))
            .Select(l => l.Split('=', 2))
            .ToDictionary(parts => parts[0], parts => parts[1].Trim(), StringComparer.OrdinalIgnoreCase);

        string Get(string key) => settings.GetValueOrDefault(key, "");

        var (width, height) = ParseDimension(Get("SIZE"));
        var (gap, gapOffset) = ParseDimension(Get("GAP"));
        var (shiftX, shiftY) = ParseCoordinates(Get("REFERENCE"));

        return new PrinterInfo(
            Speed: ParseInt(Get("SPEED")),
            Density: ParseInt(Get("DENSITY")),
            PaperWidth: width,
            PaperHeight: height,
            MediaSensor: Get("SENSOR"),
            Gap: gap,
            GapOffset: gapOffset,
            PostPrintAction: Get("POSTACTION"),
            Direction: Get("DIRECTION"),
            Offset: ParseInt(Get("SHIFT")),
            ShiftX: shiftX,
            ShiftY: shiftY,
            Ribbon: Get("RIBBON"),
            RibbonSensor: Get("RIBBONSENSOR"),
            CodePage: Get("CODEPAGE"),
            CountryCode: Get("COUNTRY")
        );
    }

    private (int a, int b) ParseDimension(string value)
    {
        var parts = value.Split(',');
        if (parts.Length < 2) return (0, 0);
        return (ParseMm(parts[0]), ParseMm(parts[1]));
    }

    private int ParseMm(string value)
    {
        value = value.Trim();
        if (value.EndsWith("inch", StringComparison.OrdinalIgnoreCase))
        {
            var num = value[..^4].Trim();
            if (double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out var inches))
                return (int)Math.Round(inches * 25.4);
        }
        else if (value.EndsWith("mm", StringComparison.OrdinalIgnoreCase))
        {
            var num = value[..^2].Trim();
            if (double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out var mm))
                return (int)Math.Round(mm);
        }
        else if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var raw))
        {
            return (int)raw;
        }
        return 0;
    }

    private (int x, int y) ParseCoordinates(string value)
    {
        var parts = value.Split(',');
        if (parts.Length < 2) return (0, 0);
        return (ParseInt(parts[0]), ParseInt(parts[1]));
    }

    private int ParseInt(string? value)
    {
        if (int.TryParse(value?.Trim(), out var result)) return result;
        return 0;
    }

    public async Task<string> SendPrinterCommand(IPAddress ip, string command)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(ip, 9100);
    
        using var stream = client.GetStream();
    
        var data = Encoding.ASCII.GetBytes(command + "\r\n");
        await stream.WriteAsync(data);
    
        await Task.Delay(500);
        var buffer = new byte[4096];
        var bytesRead = await stream.ReadAsync(buffer);
    
        return Encoding.ASCII.GetString(buffer, 0, bytesRead);
    }
}