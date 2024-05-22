using ShareLib;

namespace Domain.UseCases;
public class MakeCreditOperationEvent : Event
{
    public MakeCreditOperationEvent(Guid id, string operationCode, double value, DateTime? operationDate, OperationTypeEnum operationType)
    {
        Id = id;
        OperationCode = operationCode;
        Value = value;
        OperationDate = operationDate;
        OperationType = operationType;
    }

    public Guid Id { get; private set; }
    public string OperationCode { get; private set; }
    public OperationTypeEnum OperationType { get; private set; }
    public double Value { get; private set; }
    public DateTime? OperationDate { get; private set; }    
}
