using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReservationService.SubscriberFailed.Data;
using ReservationService.SubscriberFailed.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
        services.AddSingleton<MongoDbService>();
        services.AddSingleton<Subscriber>();
    })
.Build();

var service = host.Services.GetRequiredService<Subscriber>();
await service.StartAsync();
