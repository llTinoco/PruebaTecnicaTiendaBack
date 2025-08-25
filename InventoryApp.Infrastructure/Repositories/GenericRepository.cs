namespace InventoryApp.Infrastructure.Repositories;

using Dapper;
using InventoryApp.Domain.Repositories;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly IConfiguration _configuration;
    protected readonly string _tableName;

    protected GenericRepository(IConfiguration configuration, string tableName)
    {
        _configuration = configuration;
        _tableName = tableName;
    }

    protected IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<T>($"SELECT * FROM {_tableName} WHERE \"IsActive\" = true");
    }

    public virtual async Task<T> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<T>($"SELECT * FROM {_tableName} WHERE \"Id\" = @Id", new { Id = id });
    }

    public abstract Task<int> CreateAsync(T entity);
    public abstract Task<bool> UpdateAsync(T entity);

    public virtual async Task<bool> DeleteAsync(int id)
    {
        using var connection = CreateConnection();
        var affected = await connection.ExecuteAsync($"UPDATE {_tableName} SET \"IsActive\" = false WHERE \"Id\" = @Id", new { Id = id });
        return affected > 0;
    }
}