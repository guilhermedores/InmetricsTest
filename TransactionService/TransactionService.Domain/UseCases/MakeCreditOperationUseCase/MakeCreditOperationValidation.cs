using FluentValidation;
using ShareLib;
using TransactionService.Domain;

namespace Domain.UseCases;

public class MakeCreditOperationValidation : AbstractValidator<MakeCreditOperationCommand>
{    
    public MakeCreditOperationValidation()
    {
        RuleFor(c => c.MessageType).NotEmpty().WithMessage(ErrorMessages.FieldRequired);
        RuleFor(c => c.OperationCode).NotEmpty().WithMessage(ErrorMessages.FieldRequired);
    }
}