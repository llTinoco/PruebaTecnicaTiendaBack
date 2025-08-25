using AutoMapper;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using InventoryApp.Domain.Entities;
using InventoryApp.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> GetUserById(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<AuthResponse> Authenticate(AuthRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || user.PasswordHash != request.Password)
        {
            return new AuthResponse();
        }

        var token = GenerateJwtToken(user);
        var response = _mapper.Map<AuthResponse>(user);
        response.Token = token;
        return response;
    }

    public async Task<Result<UserDto>> CreateUser(CreateUserDto userDto)
    {
        var userExists = await _userRepository.IsUserExistsAsync(userDto.Username, userDto.FullName);
        if (userExists)
        {
            return Result<UserDto>.Fail("El nombre de usuario ya está en uso");
        }

        var user = _mapper.Map<User>(userDto);
        user.PasswordHash = userDto.Password;
        var id = await _userRepository.CreateAsync(user);
        var createdUser = await _userRepository.GetByIdAsync(id);
        var userDtoResult = _mapper.Map<UserDto>(createdUser);
        return Result<UserDto>.Ok(userDtoResult, "Usuario creado con éxito");
    }

    public async Task<Result<UserDto>> UpdateUser(UpdateUserDto userDto)
    {
        var user = await _userRepository.GetByIdAsync(userDto.Id);
        if (user == null)
        {
            return Result<UserDto>.Fail("Usuario no encontrado");
        }

        var updatedUser = _mapper.Map(userDto, user);
        var success = await _userRepository.UpdateAsync(updatedUser);

        if (!success)
        {
            return Result<UserDto>.Fail("Error al actualizar el usuario");
        }

        var result = await _userRepository.GetByIdAsync(userDto.Id);
        var userDtoResult = _mapper.Map<UserDto>(result);
        return Result<UserDto>.Ok(userDtoResult, "Usuario actualizado con éxito");
    }

    public async Task<Result> DeleteUser(int id)
    {
        var success = await _userRepository.DeleteAsync(id);
        return success
            ? Result.Ok("Usuario eliminado con éxito")
            : Result.Fail("Error al eliminar el usuario");
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
        {
            throw new InvalidOperationException("JWT configuration is missing");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtIssuer,
            Audience = jwtAudience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}