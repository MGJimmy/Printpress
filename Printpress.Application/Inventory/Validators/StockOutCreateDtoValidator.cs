using FluentValidation;

namespace Printpress.Application;

public class StockOutCreateDtoValidator : AbstractValidator<StockOutCreateDto>
{
    public StockOutCreateDtoValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(StockOutCreateDto.InventoryItemId)));

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage(ResponseMessage.MustBePositive(nameof(StockOutCreateDto.Quantity)));

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage(ResponseMessage.MaxLength(nameof(StockOutCreateDto.Notes), 500))
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
