namespace InventoryApp.Domain.Repositories;

using InventoryApp.Domain.Entities;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> IsUserExistsAsync(string username, string fullname);
}