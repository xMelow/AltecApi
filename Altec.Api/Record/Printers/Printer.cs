using System.Net;

namespace Altec.Api.Record.Printers;

public record Printer(string DnsName, string IpAddress, string model, int Port);