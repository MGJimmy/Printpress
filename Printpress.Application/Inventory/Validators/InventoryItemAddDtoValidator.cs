using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

public class InventoryItemAddDtoValidator : AbstractValidator<InventoryItemAddDto>
{
    public InventoryItemAddDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.InventoryItemCategory)
            .NotEmpty()
            .Must(value => EnumHelper.IsValidEnumValue(typeof(InventoryItemCategoryEnum), value))
            .WithMessage(ResponseMessage.CreateInvalidEnumValueMessage(
                typeof(InventoryItemCategoryEnum),
                nameof(InventoryItemAddDto.InventoryItemCategory)));

        RuleFor(x => x.ExpectedPurchaseLossPercent)
            .InclusiveBetween(0, 100);

        RuleFor(x => x.ExpectedProductionWastePercent)
            .InclusiveBetween(0, 100);

        When(x => x.PacksPerCarton.HasValue, () =>
            RuleFor(x => x.PacksPerCarton).GreaterThan(0));

        When(x => x.UnitsPerPack.HasValue, () =>
            RuleFor(x => x.UnitsPerPack).GreaterThan(0));
    }
}
