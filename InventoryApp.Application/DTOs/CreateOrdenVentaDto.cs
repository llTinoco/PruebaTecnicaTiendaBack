namespace InventoryApp.Application.DTOs;

public class CreateOrdenVentaDto
{
    public string OrderNumber { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public List<CreateDetallesOrdenesVentaDto> DetallesOrdenesVenta { get; set; } = new();
}

