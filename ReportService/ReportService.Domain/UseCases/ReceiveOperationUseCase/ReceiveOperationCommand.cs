using ShareLib;

namespace Domain.UseCases;

public class ReceiveOperationCommand : Command
{
    public ReceiveOperationCommand(string operationCode, OperationTypeEnum operationType, double value, DateTime? operationDate)
    {
        OperationCode = operationCode;
        OperationType = operationType;
        Value = value;
        OperationDate = operationDate;
    }

    public string OperationCode { get; protected set; }
    public OperationTypeEnum OperationType { get; protected set; }
    public double Value { get; protected set; }
    public DateTime? OperationDate { get; protected set; }

    public override bool IsValid()
    {
        return true;
    }
}