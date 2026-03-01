using FluentValidation;
using Printpress.Domain;

namespace Printpress.Application;

public class InventoryItemAddDtoValidator : AbstractValidator<InventoryItemAddDto>
{
    public InventoryItemAddDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(InventoryItemAddDto.Name)))
            .MaximumLength(200)
            .WithMessage(ResponseMessage.MaxLength(nameof(InventoryItemAddDto.Name), 200));

        RuleFor(x => x.InventoryItemCategory)
            .NotEmpty()
            .WithMessage(ResponseMessage.Required(nameof(InventoryItemAddDto.InventoryItemCategory)))
            .Must(value => EnumHelper.IsValidEnumValue(typeof(InventoryItemCategoryEnum), value))
            .WithMessage(ResponseMessage.CreateInvalidEnumValueMessage(
                typeof(InventoryItemCategoryEnum),
                nameof(InventoryItemAddDto.InventoryItemCategory)));

        RuleFor(x => x.ExpectedPurchaseLossPercent)
            .InclusiveBetween(0, 100)
            .WithMessage(ResponseMessage.MustBeBetween(nameof(InventoryItemAddDto.ExpectedPurchaseLossPercent), 0, 100));

        RuleFor(x => x.ExpectedProductionWastePercent)
            .InclusiveBetween(0, 100)
            .WithMessage(ResponseMessage.MustBeBetween(nameof(InventoryItemAddDto.ExpectedProductionWastePercent), 0, 100));

        When(x => x.PacksPerCarton.HasValue, () =>
            RuleFor(x => x.PacksPerCarton)
                .GreaterThan(0)
                .WithMessage(ResponseMessage.MustBePositive(nameof(InventoryItemAddDto.PacksPerCarton))));

        When(x => x.UnitsPerPack.HasValue, () =>
            RuleFor(x => x.UnitsPerPack)
                .GreaterThan(0)
                .WithMessage(ResponseMessage.MustBePositive(nameof(InventoryItemAddDto.UnitsPerPack))));
    }
}
