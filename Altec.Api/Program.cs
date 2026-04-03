using System.Text.Json;
using Altec.Api.Domain.Printers;
using Altec.Api.Domain.Tspl;
using Altec.Api.Interface;
using Altec.Api.Services;
using Altec.Api.Services.NiceLabel;
using Altec.Api.Services.Printers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddOpenApi();

builder.Services.AddScoped<TsplParser>();
builder.Services.AddScoped<TsplRender>();
builder.Services.AddScoped<TsplValidator>();
builder.Services.AddScoped<ITsplService, TsplService>();
builder.Services.AddScoped<PrinterDiscovery>();
builder.Services.AddScoped<IPrinterService, PrinterService>();

builder.Services.AddHttpClient<INiceLabelClient, NiceLabelClient>(client =>
{
    client.BaseAddress =  new Uri(builder.Configuration["NiceLabelApi:BaseUrl"]);
    client.DefaultRequestVersion = new Version(1, 1);
    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = 
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();