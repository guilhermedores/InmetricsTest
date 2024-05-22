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

        services.AddScoped<INotificationHandler<ReceiveOperationCommand>, ReceiveOperationCommandHandler>();
        services.AddHostedService<RabbitMqManager>();

        services.AddScoped<IMediatorHandler, InMemoryBus>();
        services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddDbContext<InMemoryDbContext>();

        return services;
    }
}
