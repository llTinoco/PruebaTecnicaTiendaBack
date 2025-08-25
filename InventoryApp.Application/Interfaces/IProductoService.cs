using InventoryApp.Application.DTOs;
using InventoryApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryApp.Application.Interfaces;

public interface IProductoService
{
    Task<IEnumerable<ProductoDto>> GetAllProductos();
    Task<ProductoDto> GetProductoById(int id);
    Task<Result<ProductoDto>> CreateProducto(CreateProductoDto dto);
    Task<Result<ProductoDto>> UpdateProducto(UpdateProductoDto dto);
    Task<Result> DeleteProducto(int id);
    Task<IEnumerable<ProductoDto>> GetProductosByCategoria(int categoriaId);
    Task<Result> UpdateStock(int productoId, int quantity);
}