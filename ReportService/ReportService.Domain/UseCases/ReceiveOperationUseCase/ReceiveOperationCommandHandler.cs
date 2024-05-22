using Domain.Entities;
using MediatR;
using ShareLib;

namespace Domain.UseCases;

public class ReceiveOperationCommandHandler : CommandHandler, INotificationHandler<ReceiveOperationCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly IGenericRepository<Operation> _operationRepository;

    public ReceiveOperationCommandHandler(IUnitOfWork uow, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IGenericRepository<Operation> repository) : base(uow, bus, notifications)
    {
        _uow = uow;
        _operationRepository = repository;
    }

    public Task Handle(ReceiveOperationCommand notification, CancellationToken cancellationToken)
    {
        var operation = new Operation(notification.OperationCode, notification.OperationType, notification.Value, notification.OperationDate);

        _operationRepository.Insert(operation);

        _uow.Commit();

        return Task.CompletedTask;
    }
}