namespace InventoryApp.Domain.Entities;

public class DetallesOrdenesVenta
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    
    // Propiedades de navegación
    public OrdenesVenta? Order { get; set; }
    public Producto? Product { get; set; }
}