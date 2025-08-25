using Dapper;
using InventoryApp.Domain.Entities;
using InventoryApp.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryApp.Infrastructure.Repositories;

public class OrdenesVentaRepository : GenericRepository<OrdenesVenta>, IOrdenVentaRepository
{
    public OrdenesVentaRepository(IConfiguration configuration) 
        : base(configuration, "OrdenesVenta")
    {
    }

    public async Task<OrdenesVenta> GetWithDetailsAsync(int id)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT o.*, u.""Username"", u.""FullName"" 
                   FROM ""OrdenesVenta"" o
                   LEFT JOIN ""Usuarios"" u ON o.""UserId"" = u.""Id""
                   WHERE o.""Id"" = @Id;
                   
                   SELECT d.*, p.""Name"", p.""SKU"", p.""Price""
                   FROM ""DetallesOrdenesVenta"" d
                   LEFT JOIN ""Productos"" p ON d.""ProductId"" = p.""Id""
                   WHERE d.""OrderId"" = @Id";

        using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });
        var orden = await multi.ReadFirstOrDefaultAsync<OrdenesVenta>();
        
        if (orden != null)
        {
            var detalles = (await multi.ReadAsync<DetallesOrdenesVenta>()).ToList();
            orden.DetallesOrdenesVenta = detalles;
        }
        
        return orden;
    }

    public async Task<int> CreateOrderWithDetailsAsync(OrdenesVenta orden, IEnumerable<DetallesOrdenesVenta> detalles)
    {
        using var connection = CreateConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Insertar orden
            var sqlOrden = @"INSERT INTO ""OrdenesVenta"" (""OrderNumber"", ""UserId"", ""OrderDate"", 
                           ""TotalAmount"", ""Status"", ""CustomerName"")
                           VALUES (@OrderNumber, @UserId, @OrderDate, @TotalAmount, @Status, @CustomerName)
                           RETURNING ""Id""";
            
            var ordenId = await connection.ExecuteScalarAsync<int>(sqlOrden, orden, transaction);
            
            // Insertar detalles
            var sqlDetalle = @"INSERT INTO ""DetallesOrdenesVenta"" (""OrderId"", ""ProductId"", ""Quantity"", 
                             ""UnitPrice"", ""Subtotal"")
                             VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @Subtotal)";
            
            foreach (var detalle in detalles)
            {
                detalle.OrderId = ordenId;
                await connection.ExecuteAsync(sqlDetalle, detalle, transaction);
                
                // Actualizar stock
                await connection.ExecuteAsync(
                    @"UPDATE ""Productos"" SET ""Stock"" = ""Stock"" - @Quantity WHERE ""Id"" = @ProductId", 
                    new { detalle.Quantity, detalle.ProductId }, 
                    transaction);
            }
            
            transaction.Commit();
            return ordenId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public override async Task<int> CreateAsync(OrdenesVenta entity)
    {
        using var connection = CreateConnection();
        var sql = @"INSERT INTO ""OrdenesVenta"" (""OrderNumber"", ""UserId"", ""OrderDate"", 
                   ""TotalAmount"", ""Status"", ""CustomerName"")
                   VALUES (@OrderNumber, @UserId, @OrderDate, @TotalAmount, @Status, @CustomerName)
                   RETURNING ""Id""";
        
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public override async Task<bool> UpdateAsync(OrdenesVenta entity)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE ""OrdenesVenta"" 
                   SET ""Status"" = @Status,
                       ""CustomerName"" = @CustomerName
                   WHERE ""Id"" = @Id";
        
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }
}