using FluentValidation;
using TransactionService.Domain;

namespace Domain.UseCases;

public class MakeDebitOperationValidation : AbstractValidator<MakeDebitOperationCommand>
{    
    public MakeDebitOperationValidation()
    {
        RuleFor(c => c.MessageType).NotEmpty().WithMessage(ErrorMessages.FieldRequired);
        RuleFor(c => c.OperationCode).NotEmpty().WithMessage(ErrorMessages.FieldRequired);
    }
}