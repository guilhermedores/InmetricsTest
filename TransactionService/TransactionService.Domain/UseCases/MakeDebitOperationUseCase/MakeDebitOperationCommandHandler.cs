using Domain.Entities;
using MediatR;
using ShareLib;

namespace Domain.UseCases;

public class MakeDebitOperationCommandHandler : CommandHandler,INotificationHandler<MakeDebitOperationCommand>
{
    private readonly IMediatorHandler _bus;
    private readonly IUnitOfWork _uow;
    private readonly IGenericRepository<Operation> _operationRepository;

    public MakeDebitOperationCommandHandler(IUnitOfWork uow, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IGenericRepository<Operation> repository) : base(uow, bus, notifications)
    {
        _bus = bus;
        _uow = uow;
        _operationRepository = repository;
    }

    public Task Handle(MakeDebitOperationCommand notification, CancellationToken cancellationToken)
    {
        if (!notification.IsValid())
        {
            NotifyValidationErrors(notification);
            return Task.CompletedTask;
        };

        var operation = new Operation(notification.OperationCode, OperationTypeEnum.Debit, notification.Value, DateTime.Now);

        _operationRepository.Insert(operation);

        if (_uow.Commit())
        {
            _bus.RaiseEvent(new MakeDebitOperationEvent(operation.Id,
                                                         operation.OperationCode,
                                                         operation.Value,
                                                         operation.OperationDate,
                                                         operation.OperationType));
        }

        return Task.CompletedTask;
    }
}