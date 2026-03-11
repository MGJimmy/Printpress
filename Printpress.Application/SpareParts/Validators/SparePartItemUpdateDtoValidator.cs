using FluentValidation;

namespace Printpress.Application;

public class SparePartItemUpdateDtoValidator : AbstractValidator<SparePartItemUpdateDto>
{
    public SparePartItemUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ResponseMessage.Required(nameof(SparePartItemUpdateDto.Name)))
            .MaximumLength(200).WithMessage(ResponseMessage.MaxLength(nameof(SparePartItemUpdateDto.Name), 200));

        When(x => x.PacksPerCarton.HasValue, () =>
            RuleFor(x => x.PacksPerCarton).GreaterThan(0).WithMessage(ResponseMessage.MustBePositive(nameof(SparePartItemUpdateDto.PacksPerCarton))));

        When(x => x.UnitsPerPack.HasValue, () =>
            RuleFor(x => x.UnitsPerPack).GreaterThan(0).WithMessage(ResponseMessage.MustBePositive(nameof(SparePartItemUpdateDto.UnitsPerPack))));
    }
}
