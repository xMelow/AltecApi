using System.Net;

namespace Altec.Api.Record.Printers;

public record Printer(string DnsName, string ShortDnsName, string IpAddress, string PrinterModel, int Port);