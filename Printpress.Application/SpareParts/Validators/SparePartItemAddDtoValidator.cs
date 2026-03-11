using FluentValidation;

namespace Printpress.Application;

public class SparePartItemAddDtoValidator : AbstractValidator<SparePartItemAddDto>
{
    public SparePartItemAddDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ResponseMessage.Required(nameof(SparePartItemAddDto.Name)))
            .MaximumLength(200).WithMessage(ResponseMessage.MaxLength(nameof(SparePartItemAddDto.Name), 200));

        When(x => x.PacksPerCarton.HasValue, () =>
            RuleFor(x => x.PacksPerCarton).GreaterThan(0).WithMessage(ResponseMessage.MustBePositive(nameof(SparePartItemAddDto.PacksPerCarton))));

        When(x => x.UnitsPerPack.HasValue, () =>
            RuleFor(x => x.UnitsPerPack).GreaterThan(0).WithMessage(ResponseMessage.MustBePositive(nameof(SparePartItemAddDto.UnitsPerPack))));
    }
}
