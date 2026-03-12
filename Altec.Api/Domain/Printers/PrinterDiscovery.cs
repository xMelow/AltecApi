using System.Net;
using Altec.Api.Record.Printers;

namespace Altec.Api.Domain.Printers;

public class PrinterDiscovery
{
    public PrinterDiscovery() {}
    
    public async Task<IReadOnlyList<Printer>> Discover(List<string> subnets)
    {
        // split subnet and calculate ip range
        // scan the ip ranges
        //      see if port 9100 reports
        // return found printers
        throw new NotImplementedException();
    }

    private (IPAddress baseIp, int prefixLength) ParseSubnet(string subnet)
    {
        var parts = subnet.Split("/");
        return (IPAddress.Parse(parts[0]), Convert.ToInt32(parts[1]));
    }
}