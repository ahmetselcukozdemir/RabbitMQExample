using MassTransit;
using Microsoft.Extensions.Hosting;
using RabbitMQ.WorkerService.MassTransit.Consumer;
using RabbitMQ.WorkerService.MassTransit.Consumer.Consomer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //temel yapılandırma
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<ExampleMessageConsomer>();

            configurator.UsingRabbitMq((context, _configurator) =>
            {
                _configurator.Host("*******");

                _configurator.ReceiveEndpoint("example-message-queue", e=> e.ConfigureConsumer<ExampleMessageConsomer>(context));
            });
        });

       
    })
    .Build();

await host.RunAsync();