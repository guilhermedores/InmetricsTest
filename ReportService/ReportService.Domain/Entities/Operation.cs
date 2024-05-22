using ShareLib;

namespace Domain.Entities;
public class Operation : Entity
{
    public Operation(string operationCode, OperationTypeEnum operationType, double value, DateTime? operationDate)
    {
        OperationCode = operationCode;
        OperationType = operationType;
        Value = value;
        OperationDate = operationDate;
    }

    public string OperationCode { get; private set; } = string.Empty;
    public OperationTypeEnum OperationType { get; private set; }
    public double Value { get; private set; }
    public DateTime? OperationDate { get; private set; }
}