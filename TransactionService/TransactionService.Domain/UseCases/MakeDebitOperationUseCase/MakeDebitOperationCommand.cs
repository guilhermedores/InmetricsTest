using FluentValidation;
using ShareLib;

namespace Domain.UseCases;

public class MakeDebitOperationCommand : Command
{
    public MakeDebitOperationCommand(string operationCode, double value)
    {
        OperationCode = operationCode;
        Value = value;
    }

    public string OperationCode { get; protected set; } = string.Empty;
    public double Value { get; protected set; }

    public override bool IsValid()
    {
        ValidationResult = new MakeDebitOperationValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}