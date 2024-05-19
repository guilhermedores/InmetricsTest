using FluentValidation.Results;

namespace ShareLib;

public abstract class Command : Message
{
    public ValidationResult ValidationResult { get; set; }

    public abstract bool IsValid();
}