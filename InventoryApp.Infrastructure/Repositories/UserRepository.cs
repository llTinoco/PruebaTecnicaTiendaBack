using Dapper;
using InventoryApp.Domain.Entities;
using InventoryApp.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace InventoryApp.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(IConfiguration configuration) 
        : base(configuration, "Users")
    {
    }

    public override async Task<int> CreateAsync(User entity)
    {
        using var connection = CreateConnection();
        var sql = @"INSERT INTO ""Users"" (""Username"", ""PasswordHash"", ""FullName"")
                   VALUES (@Username, @PasswordHash, @FullName)
                   RETURNING ""Id""";
        
        
        
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public override async Task<bool> UpdateAsync(User entity)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE ""Users"" 
                   SET ""Username"" = @Username,""FullName"" = @FullName
                   WHERE ""Id"" = @Id";
        
        
        
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected > 0;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM \"Users\" WHERE \"Username\" = @Username", 
            new { Username = username });
    }

    public async Task<bool> IsUserExistsAsync(string username, string fullname)
    {
        using var connection = CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM \"Users\" WHERE \"Username\" = @Username OR \"FullName\" = @FullName",
            new { Username = username, FullName = fullname });

        return count > 0;
    }
}