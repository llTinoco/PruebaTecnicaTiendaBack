using AutoMapper;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using InventoryApp.Domain.Entities;
using InventoryApp.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryApp.Application.Services;

public class OrdenVentaService : IOrdenVentaService
{
    private readonly IOrdenVentaRepository _ordenVentaRepository;
    private readonly IMapper _mapper;

    public OrdenVentaService(IOrdenVentaRepository ordenVentaRepository, IMapper mapper)
    {
        _ordenVentaRepository = ordenVentaRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrdenesVentaDto>> GetAll()
    {
        var ordenes = await _ordenVentaRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<OrdenesVentaDto>>(ordenes);
    }

    public async Task<OrdenesVentaDto?> GetById(int id)
    {
        var orden = await _ordenVentaRepository.GetWithDetailsAsync(id);
        return _mapper.Map<OrdenesVentaDto>(orden);
    }

    public async Task<Result<OrdenesVentaDto>> Create(CreateOrdenVentaDto dto)
    {
        var orden = _mapper.Map<OrdenesVenta>(dto);
        var detalles = _mapper.Map<List<DetallesOrdenesVenta>>(dto.DetallesOrdenesVenta);
        var id = await _ordenVentaRepository.CreateOrderWithDetailsAsync(orden, detalles);
        var created = await _ordenVentaRepository.GetWithDetailsAsync(id);
        return Result<OrdenesVentaDto>.Ok(_mapper.Map<OrdenesVentaDto>(created), "Orden creada con éxito");
    }

    public async Task<Result> Delete(int id)
    {
        var deleted = await _ordenVentaRepository.DeleteAsync(id);
        return deleted ? Result.Ok("Orden eliminada") : Result.Fail("No se pudo eliminar la orden");
    }
}