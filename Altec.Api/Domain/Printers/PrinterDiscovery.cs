using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Altec.Api.Record.Printers;

namespace Altec.Api.Domain.Printers;

public class PrinterDiscovery
{
    public PrinterDiscovery() {}
    
    public async Task<IReadOnlyList<Printer>> Discover(List<string> subnets)
    {
        List<Printer> printers = new List<Printer>();
        
        foreach (var subnet in subnets)
        {
            const int printerPort = 9100;
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
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var tasks = ipAddresses.Select(ip => IsPortOpen(ip, printerPort, 200));
            var results = await Task.WhenAll(tasks);
            Console.WriteLine($"Port scan took: {sw.ElapsedMilliseconds}ms");
            var foundIps = ipAddresses
                .Zip(results, (ip, isOpen) => (ip, isOpen))
                .Where(x => x.isOpen)
                .Select(x => x.ip);

            sw.Restart();
            var printerTask = foundIps.Select(async ip =>
            {
                try
                {
                    var printerInfo = await GetPrinterInfo(ip, printerPort);
                    return new Printer(printerInfo.printerDnsName, ip.ToString(), printerInfo.printerModelName, printerPort);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error for {ip}: {ex.Message}");
                    return null;
                }
            });
            var foundPrinters = await Task.WhenAll(printerTask);
            Console.WriteLine($"Printer info took: {sw.ElapsedMilliseconds}ms");

            printers.AddRange(foundPrinters.Where(p => p != null));
            
            
        }
        return printers;
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
        catch (Exception ex)
        {
            Console.WriteLine($"IsPortOpen error for {ip}: {ex.Message}");
            return false;
        }
    }

    private async Task<(string printerDnsName, string printerModelName)> GetPrinterInfo(IPAddress ip, int port)
    {
        var dnsTask = GetPrinterDnsName(ip);
        var modelTask = GetPrinterModelName(ip, port);
        await Task.WhenAll(dnsTask, modelTask);
        return (await dnsTask, await modelTask);
    }

    private async Task<string> GetPrinterDnsName(IPAddress ip)
    {
        try
        {
            var dnsTask = Dns.GetHostEntryAsync(ip);
            var timeoutTask = Task.Delay(200);
            var completed = await Task.WhenAny(dnsTask, timeoutTask);
            if (completed == dnsTask)
                return (await dnsTask).HostName;
            return ip.ToString();
        }
        catch
        {
            return ip.ToString();
        }
    }

    private async Task<string> GetPrinterModelName(IPAddress ip, int port)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(ip, port);
            var stream = client.GetStream();
            byte[] command = Encoding.ASCII.GetBytes("~!T\r\n");
            await stream.WriteAsync(command, 0, command.Length);
            
            await Task.Delay(100);
            
            byte[] buffer = new byte[1024];
            var readTask = stream.ReadAsync(buffer, 0, buffer.Length);
            var timeoutTask = Task.Delay(200);
            var completed = await Task.WhenAny(readTask, timeoutTask);
        
            if (completed == readTask)
            {
                var bytesRead = await readTask;
                return Encoding.ASCII.GetString(buffer, 0, bytesRead);
            }
            return "Unknown";
        }
        catch
        {
            // convert to issue
            return "Can't get printer model name";
        }
    }
}