namespace InventoryApp.Application.DTOs;

public class ProductoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public int ProveedorId { get; set; }
    public string ProveedorNombre { get; set; } = string.Empty;
}

public class CreateProductoDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoriaId { get; set; }
    public int ProveedorId { get; set; }
}

public class UpdateProductoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoriaId { get; set; }
    public int ProveedorId { get; set; }
}