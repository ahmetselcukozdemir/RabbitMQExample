using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Hosting;
using RabbitMQ.WorkerService.MassTransit.Publisher;
using RabbitMQ.WorkerService.MassTransit.Publisher.Services;
using Microsoft.Extensions.DependencyInjection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //temel yapılandırma
        services.AddMassTransit(configurator =>
        {
            configurator.UsingRabbitMq((context, _configurator) =>
            {
                _configurator.Host("*******");
            });
        });

        //servisimizin referansını bildiriyoruz.
        services.AddHostedService<PublishMessageService>(provider =>
        {
            //worker servislerde masstransit kullanıyorken inject hatası için bu eklemeyi yapýyoruz.
            using IServiceScope scope = provider.CreateScope();
            IPublishEndpoint publishEndpoint = scope.ServiceProvider.GetService<IPublishEndpoint>();
            return new(publishEndpoint);
        });
    })
    .Build();

host.Run();