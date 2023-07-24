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

        services.AddHostedService<PublishMessageService>();
    })
    .Build();

host.Run();
