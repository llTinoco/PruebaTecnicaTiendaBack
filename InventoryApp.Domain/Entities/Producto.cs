namespace InventoryApp.Domain.Entities;

public class Producto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoriaId { get; set; }
    public int ProveedorId { get; set; }
    
    // Propiedades de navegación (para relaciones)
    public Categoria? Categoria { get; set; }
    public Proveedor? Proveedor { get; set; }
}