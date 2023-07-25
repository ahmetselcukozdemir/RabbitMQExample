using MassTransit;
using MassTransit.Testing;
using RabbitMQ.WorkerService.MassTransit.Publisher;
using RabbitMQ.WorkerService.MassTransit.Publisher.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //temel yap�land�rma
        services.AddMassTransit(configurator =>
        {
            configurator.UsingRabbitMq((context, _configurator) =>
            {
                _configurator.Host("*******");
            });
        });

        //servisimizin referans�n�n bildiriyoruz.
        services.AddHostedService<PublishMessageService>(provider =>
        {
            //worker servislerde masstransit kullan�rken inject hatas� i�in bu eklemeyi yap�yoruz.
            using IServiceScope scope = provider.CreateScope();
            IPublishEndpoint publishEndpoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
            return new(publishEndpoint);
        });
    })
    .Build();

host.Run();
