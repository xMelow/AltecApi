using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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
                ipAddresses.Add(ipAddress);
            }
            var tasks = ipAddresses.Select(ip => IsPortOpen(ip, printerPort, 500));
            var results = await Task.WhenAll(tasks);
            var foundIps = ipAddresses
                .Zip(results, (ip, isOpen) => (ip, isOpen))
                .Where(x => x.isOpen)
                .Select(x => x.ip);

            foreach (var ip in foundIps)
            {
                printers.Add(new Printer(
                    Name: ip.ToString(),
                    HostnameOrIpAddress: ip.ToString(),
                    Port: printerPort
                ));
            }
        }
        return printers;
    }

    private (IPAddress baseIp, int prefixLength) ParseSubnet(string subnet)
    {
        var parts = subnet.Split("/");
        return (IPAddress.Parse(parts[0]), Convert.ToInt32(parts[1]));
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
}