using Domain.Entities;
using MediatR;
using ShareLib;

namespace Domain.UseCases;

public class MakeCreditOperationCommandHandler : CommandHandler, INotificationHandler<MakeCreditOperationCommand>
{
    private readonly IMediatorHandler _bus;
    private readonly IUnitOfWork _uow;
    private readonly IGenericRepository<Operation> _operationRepository;

    public MakeCreditOperationCommandHandler(IUnitOfWork uow, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IGenericRepository<Operation> repository) : base(uow, bus, notifications)
    {
        _bus = bus;
        _uow = uow;
        _operationRepository = repository;
    }

    public Task Handle(MakeCreditOperationCommand notification, CancellationToken cancellationToken)
    {
        if (!notification.IsValid())
        {
            NotifyValidationErrors(notification);
            return Task.CompletedTask;
        };

        var operation = new Operation(notification.OperationCode, OperationTypeEnum.Credit, notification.Value, DateTime.Now);

        _operationRepository.Insert(operation);

        if (_uow.Commit())
        {
            _bus.RaiseEvent(new MakeCreditOperationEvent(operation.Id,
                                                         operation.OperationCode,
                                                         operation.Value,
                                                         operation.OperationDate,
                                                         operation.OperationType));
        }

        return Task.CompletedTask;
    }
}