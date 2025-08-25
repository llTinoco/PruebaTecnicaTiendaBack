namespace InventoryApp.Domain.Entities;

public class OrdenesVenta
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
   
    // Propiedades de navegación
    public User? User { get; set; }
    public List<DetallesOrdenesVenta> DetallesOrdenesVenta { get; set; } = new List<DetallesOrdenesVenta>();
}