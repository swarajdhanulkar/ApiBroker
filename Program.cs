using ApiBroker;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ProviderSelector>(sp =>
{
    var providers = new List<ProviderMetrics>
    {
        new() { ProviderName = "IpInfo", BaseUrl = "https://ipinfo.io" },
        new() { ProviderName = "IpApi", BaseUrl = "https://ip-api.com/json" },
        new() { ProviderName = "Ipwhois", BaseUrl = "https://ipwhois.app/json/" },
    };
    return new ProviderSelector(providers, maxPerMin: 60);
});

builder.Services.AddHttpClient<LocationService>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
