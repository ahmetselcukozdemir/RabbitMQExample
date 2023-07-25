using MassTransit;
using MassTransit.Testing;
using RabbitMQ.WorkerService.MassTransit.Publisher;
using RabbitMQ.WorkerService.MassTransit.Publisher.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //temel yapýlandýrma
        services.AddMassTransit(configurator =>
        {
            configurator.UsingRabbitMq((context, _configurator) =>
            {
                _configurator.Host("*******");
            });
        });

        //servisimizin referansýnýn bildiriyoruz.
        services.AddHostedService<PublishMessageService>(provider =>
        {
            //worker servislerde masstransit kullanýrken inject hatasý için bu eklemeyi yapýyoruz.
            using IServiceScope scope = provider.CreateScope();
            IPublishEndpoint publishEndpoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
            return new(publishEndpoint);
        });
    })
    .Build();

host.Run();
