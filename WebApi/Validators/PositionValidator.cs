using FluentValidation;
using WebApi.DTOs;

namespace WebApi.Validators
{
    public class PositionValidator : AbstractValidator<PositionDTO>
    {
        public PositionValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es obligatorio")
                .MaximumLength(100).WithMessage("El título no puede tener más de 100 caracteres");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria")
                .MaximumLength(1000).WithMessage("Máximo 1000 caracteres en la descripción");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("La ubicación es obligatoria");

            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(s => new[] { "draft", "open", "closed", "archived" }.Contains(s))
                .WithMessage("El estado debe ser: draft, open, closed o archived");

            RuleFor(x => x.RecruiterId)
                .GreaterThan(0).WithMessage("RecruiterId debe ser mayor a cero");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("DepartmentId debe ser mayor a cero");

            RuleFor(x => x.Budget)
                .GreaterThan(0).WithMessage("El presupuesto debe ser mayor a cero");
        }
    }
}
