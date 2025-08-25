namespace InventoryApp.Domain.Repositories;

using InventoryApp.Domain.Entities;

public interface IProductoRepository : IGenericRepository<Producto>
{
    Task<IEnumerable<Producto>> GetByCategoryAsync(int categoriaId);
    Task<IEnumerable<Producto>> GetBySupplierAsync(int proveedorId);
    Task<bool> UpdateStockAsync(int productoId, int quantity);
}