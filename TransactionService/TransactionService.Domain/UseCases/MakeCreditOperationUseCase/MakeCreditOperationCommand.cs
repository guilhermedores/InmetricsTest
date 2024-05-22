using ShareLib;

namespace Domain.UseCases;

public class MakeCreditOperationCommand : Command
{
    public MakeCreditOperationCommand(string operationCode, double value)
    {
        OperationCode = operationCode;
        Value = value;
    }

    public string OperationCode { get; protected set; } = string.Empty;
    public double Value { get; protected set; }

    public override bool IsValid()
    {
        ValidationResult = new MakeCreditOperationValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}