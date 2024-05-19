using ShareLib;

namespace Domain.UseCases;

public class MakeCreditOperationCommand : Command
{
    public string OperationCode { get; protected set; } = string.Empty;
    public double Value { get; protected set; }

    public override bool IsValid()
    {
        throw new NotImplementedException();
    }
}