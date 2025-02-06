using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Meta;
using ReservationService;
using ReservationService.Subscriber.Services;
using ReservationService.SubscriberSuccess.Services;


var factory = new DesignTimeDbContextFactory();

using var dbContext = factory.CreateDbContext(args);

if (!await dbContext.Database.CanConnectAsync())
{
    await dbContext.Database.MigrateAsync();
    //Console.WriteLine("Database is set up and ready!");
}

await ConstraintApplier.ApplyReservationConstraints(dbContext);

var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                services.AddSingleton(configuration);
                services.AddSingleton<Subscriber>();
            })
            .Build();

var service = host.Services.GetRequiredService<Subscriber>();
await service.StartAsync();