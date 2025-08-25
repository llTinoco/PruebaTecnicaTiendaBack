using InventoryApp.Application.DTOs;
using InventoryApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryApp.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<UserDto> GetUserById(int id);
    Task<AuthResponse> Authenticate(AuthRequest request);
    Task<Result<UserDto>> CreateUser(CreateUserDto userDto);
    Task<Result<UserDto>> UpdateUser(UpdateUserDto userDto);
    Task<Result> DeleteUser(int id);
}