using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

public class InventoryItemUpdateDtoValidator : AbstractValidator<InventoryItemUpdateDto>
{
    public InventoryItemUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(InventoryItemUpdateDto.Name)))
            .MaximumLength(200)
            .WithMessage(ResponseMessage.MaxLength(nameof(InventoryItemUpdateDto.Name), 200));

        RuleFor(x => x.InventoryItemCategory)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(InventoryItemUpdateDto.InventoryItemCategory)))
            .Must(value => EnumHelper.IsValidEnumValue(typeof(InventoryItemCategoryEnum), value))
            .WithMessage(ResponseMessage.CreateInvalidEnumValueMessage(
                typeof(InventoryItemCategoryEnum),
                nameof(InventoryItemUpdateDto.InventoryItemCategory)));

        RuleFor(x => x.ExpectedPurchaseLossPercent)
            .InclusiveBetween(0, 100)
            .WithMessage(ResponseMessage.MustBeBetween(nameof(InventoryItemUpdateDto.ExpectedPurchaseLossPercent), 0, 100));

        RuleFor(x => x.ExpectedProductionWastePercent)
            .InclusiveBetween(0, 100)
            .WithMessage(ResponseMessage.MustBeBetween(nameof(InventoryItemUpdateDto.ExpectedProductionWastePercent), 0, 100));

        When(x => x.PacksPerCarton.HasValue, () =>
            RuleFor(x => x.PacksPerCarton)
                .GreaterThan(0)
                .WithMessage(ResponseMessage.MustBePositive(nameof(InventoryItemUpdateDto.PacksPerCarton))));

        When(x => x.UnitsPerPack.HasValue, () =>
            RuleFor(x => x.UnitsPerPack)
                .GreaterThan(0)
                .WithMessage(ResponseMessage.MustBePositive(nameof(InventoryItemUpdateDto.UnitsPerPack))));
    }
}
