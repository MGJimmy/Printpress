using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

public class InventoryUsageSettlementCreateDtoValidator : AbstractValidator<InventoryUsageSettlementCreateDto>
{
    public InventoryUsageSettlementCreateDtoValidator(ILocalizationService loc)
    {
        RuleFor(x => x.InventoryItemId)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Shared.Required, loc.Get(LocalizationKeys.Inventory.FieldItem)));

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage(_ => loc.Get(LocalizationKeys.Inventory.UsageSettlementQuantityMustBePositive));

        RuleFor(x => x.SettlementType)
            .IsInEnum()
            .WithMessage(_ => loc.Get(LocalizationKeys.Inventory.UsageSettlementTypeInvalid));

        RuleFor(x => x.OccurredAt)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Inventory.UsageSettlementDateRequired));

        RuleFor(x => x.Notes)
            .NotEmpty()
            .WithMessage(_ => loc.Get(LocalizationKeys.Inventory.UsageSettlementNotesRequired))
            .MaximumLength(500)
            .WithMessage(_ => loc.Get(LocalizationKeys.Inventory.UsageSettlementNotesMaxLength));
    }
}
