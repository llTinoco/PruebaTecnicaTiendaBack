namespace InventoryApp.Domain.Repositories;
using InventoryApp.Domain.Entities;

public interface IOrdenVentaRepository : IGenericRepository<OrdenesVenta>
{
    Task<OrdenesVenta> GetWithDetailsAsync(int id);
    Task<int> CreateOrderWithDetailsAsync(OrdenesVenta orden, IEnumerable<DetallesOrdenesVenta> detalles);
}