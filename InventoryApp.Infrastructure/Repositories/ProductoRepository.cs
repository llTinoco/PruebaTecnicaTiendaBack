using Dapper;
using InventoryApp.Domain.Entities;
using InventoryApp.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryApp.Infrastructure.Repositories;

public class ProductoRepository : GenericRepository<Producto>, IProductoRepository
{
    public ProductoRepository(IConfiguration configuration) 
        : base(configuration, "Productos")
    {
    }

    public override async Task<IEnumerable<Producto>> GetAllAsync()
    {
        using var connection = CreateConnection();
        var sql = @"SELECT p.*, c.""Name"" as ""CategoriaName"", pr.""Name"" as ""ProveedorName""
                   FROM ""Productos"" p
                   INNER JOIN ""Categoria"" c ON p.""CategoriaId"" = c.""Id""
                   INNER JOIN ""Proveedor"" pr ON p.""ProveedorId"" = pr.""Id""";
                   
        return await connection.QueryAsync<Producto>(sql);
    }

    public override async Task<Producto> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT p.*, c.""Name"" as ""CategoriaName"", pr.""Name"" as ""ProveedorName""
                   FROM ""Productos"" p
                   INNER JOIN ""Categoria"" c ON p.""CategoriaId"" = c.""Id""
                   INNER JOIN ""Proveedor"" pr ON p.""ProveedorId"" = pr.""Id""
                   WHERE p.""Id"" = @Id";
                   
        return await connection.QueryFirstOrDefaultAsync<Producto>(sql, new { Id = id });
    }

    public override async Task<int> CreateAsync(Producto entity)
    {
        using var connection = CreateConnection();
        var sql = @"INSERT INTO ""Productos"" (""Name"", ""Description"", ""SKU"", ""Price"", ""Stock"", ""CategoriaId"", ""ProveedorId"")
                   VALUES (@Name, @Description, @SKU, @Price, @Stock, @CategoriaId, @ProveedorId)
                   RETURNING ""Id""";
        
     
        
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public override async Task<bool> UpdateAsync(Producto entity)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE ""Productos"" 
                   SET ""Name"" = @Name, ""Description"" = @Description, ""SKU"" = @SKU,
                       ""Price"" = @Price, ""Stock"" = @Stock, ""CategoriaId"" = @CategoriaId, 
                       ""ProveedorId"" = @ProveedorId,
                   WHERE ""Id"" = @Id";
        
       
        
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<IEnumerable<Producto>> GetByCategoryAsync(int categoriaId)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT p.*, c.""Name"" as ""CategoriaName"", pr.""Name"" as ""ProveedorName""
                   FROM ""Productos"" p
                   INNER JOIN ""Categoria"" c ON p.""CategoriaId"" = c.""Id""
                   INNER JOIN ""Proveedor"" pr ON p.""ProveedorId"" = pr.""Id""
                   WHERE p.""CategoriaId"" = @CategoriaId";
                   
        return await connection.QueryAsync<Producto>(sql, new { CategoriaId = categoriaId });
    }

    public async Task<IEnumerable<Producto>> GetBySupplierAsync(int proveedorId)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT p.*, c.""Name"" as ""CategoriaName"", pr.""Name"" as ""ProveedorName""
                   FROM ""Productos"" p
                   INNER JOIN ""Categoria"" c ON p.""CategoriaId"" = c.""Id""
                   INNER JOIN ""Proveedor"" pr ON p.""ProveedorId"" = pr.""Id""
                   WHERE p.""ProveedorId"" = @ProveedorId";
                   
        return await connection.QueryAsync<Producto>(sql, new { ProveedorId = proveedorId });
    }

    public async Task<bool> UpdateStockAsync(int productoId, int quantity)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE ""Productos"" 
                   SET ""Stock"" = ""Stock"" + @Quantity, 
                       ""UpdatedAt"" = @UpdatedAt
                   WHERE ""Id"" = @Id";
        
        var affected = await connection.ExecuteAsync(sql, new { 
            Id = productoId, 
            Quantity = quantity,
            UpdatedAt = DateTime.UtcNow
        });
        
        return affected > 0;
    }
}