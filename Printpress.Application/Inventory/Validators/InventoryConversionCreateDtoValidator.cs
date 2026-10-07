using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

public class InventoryConversionCreateDtoValidator : AbstractValidator<InventoryConversionCreateDto>
{
    public InventoryConversionCreateDtoValidator(ILocalizationService loc)
    {
        RuleFor(x => x.InventoryItemId)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.Required, loc.Get(LocalizationKeys.Inventory.FieldItem)));

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage(_ => loc.Get(LocalizationKeys.Inventory.ConversionQuantityMustBePositive));

        RuleFor(x => x.OccurredAt)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Inventory.ConversionDateRequired));

        RuleFor(x => x.Notes)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Inventory.ConversionNotesRequired))
            .MaximumLength(500)
            .WithMessage(_ => loc.Get(LocalizationKeys.Inventory.ConversionNotesMaxLength));
    }
}
