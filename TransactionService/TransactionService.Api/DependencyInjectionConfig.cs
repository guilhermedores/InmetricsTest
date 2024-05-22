using Domain.Entities;
using Domain.ExternalPorts;
using Domain.UseCases;
using MediatR;
using ShareLib;

public static partial class ServiceConfig
{
    public static IServiceCollection AddMyDependencyInjection(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Startup>());

        services.AddScoped<IGenericRepository<Operation>, GenericRepository<Operation>>();

        services.AddScoped<INotificationHandler<MakeCreditOperationCommand>, MakeCreditOperationCommandHandler>();
        services.AddScoped<INotificationHandler<MakeDebitOperationCommand>, MakeDebitOperationCommandHandler>();

        services.AddScoped<INotificationHandler<MakeCreditOperationEvent>, MakeCreditOperationEventHandler>();
        services.AddScoped<INotificationHandler<MakeDebitOperationEvent>, MakeDebitOperationEventHandler>();
        services.AddScoped<IMessageQueueManager, RabbitMqManager>();



        services.AddScoped<IMediatorHandler, InMemoryBus>();
        services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddDbContext<InMemoryDbContext>();

        return services;
    }
}
