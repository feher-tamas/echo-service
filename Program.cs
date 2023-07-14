using EchoService;
using EchoService.Configuration;
using NLog.Extensions.Logging;
using NLog;
using NLog.Extensions.Hosting;

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
var enviromentVariable = environment ?? "Production";

var config = new ConfigurationBuilder()
  .AddJsonFile($"appsettings.{enviromentVariable}.json", optional: true, reloadOnChange: true)
  .AddEnvironmentVariables()
  .Build();

LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

var logger = LogManager.Setup()
                       .SetupExtensions(ext => ext.RegisterConfigSettings(config))
                       .GetCurrentClassLogger();
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddConfigs(config);
        services.AddLogging();
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging(builder => builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace))
    .UseNLog()
    .Build();

await host.RunAsync();
