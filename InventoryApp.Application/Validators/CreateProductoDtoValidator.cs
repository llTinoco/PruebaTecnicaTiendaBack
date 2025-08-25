using FluentValidation;
using InventoryApp.Application.DTOs;

namespace InventoryApp.Application.Validators;

public class CreateProductoDtoValidator : AbstractValidator<CreateProductoDto>
{
    public CreateProductoDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres");
            
        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("El SKU es requerido")
            .MaximumLength(30).WithMessage("El SKU no puede exceder los 30 caracteres");
            
        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("El precio es requerido")
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");
            
        RuleFor(x => x.Stock)
            .NotEmpty().WithMessage("El stock es requerido")
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo");
            
        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("La categoría es requerida");
            
        RuleFor(x => x.ProveedorId)
            .NotEmpty().WithMessage("El proveedor es requerido");
    }
}