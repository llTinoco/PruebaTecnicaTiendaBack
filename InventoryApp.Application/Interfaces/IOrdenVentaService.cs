using InventoryApp.Application.DTOs;
using InventoryApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryApp.Application.Interfaces;

public interface IOrdenVentaService
{
    Task<IEnumerable<OrdenesVentaDto>> GetAll();
    Task<OrdenesVentaDto?> GetById(int id);
    Task<Result<OrdenesVentaDto>> Create(CreateOrdenVentaDto dto);
    Task<Result> Delete(int id);
}