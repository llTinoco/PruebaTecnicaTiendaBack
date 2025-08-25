using AutoMapper;
using FluentValidation;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using InventoryApp.Domain.Entities;
using InventoryApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryApp.Application.Services;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProductoDto> _createValidator;
    private readonly IValidator<UpdateProductoDto> _updateValidator;

    public ProductoService(
        IProductoRepository productoRepository, 
        IMapper mapper,
        IValidator<CreateProductoDto> createValidator,
        IValidator<UpdateProductoDto> updateValidator)
    {
        _productoRepository = productoRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<ProductoDto>> GetAllProductos()
    {
        var productos = await _productoRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductoDto>>(productos);
    }

    public async Task<ProductoDto> GetProductoById(int id)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        return _mapper.Map<ProductoDto>(producto);
    }

    public async Task<Result<ProductoDto>> CreateProducto(CreateProductoDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
        {
            return Result<ProductoDto>.Fail(
                "Error de validación", 
                validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var producto = _mapper.Map<Producto>(dto);
        var id = await _productoRepository.CreateAsync(producto);
        
        var createdProducto = await _productoRepository.GetByIdAsync(id);
        var productoDto = _mapper.Map<ProductoDto>(createdProducto);
        
        return Result<ProductoDto>.Ok(productoDto, "Producto creado con éxito");
    }

    public async Task<Result<ProductoDto>> UpdateProducto(UpdateProductoDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
        {
            return Result<ProductoDto>.Fail(
                "Error de validación", 
                validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var existingProducto = await _productoRepository.GetByIdAsync(dto.Id);
        if (existingProducto == null)
        {
            return Result<ProductoDto>.Fail("Producto no encontrado");
        }

        var producto = _mapper.Map(dto, existingProducto);
        var updated = await _productoRepository.UpdateAsync(producto);
        
        if (!updated)
        {
            return Result<ProductoDto>.Fail("Error al actualizar el producto");
        }
        
        var updatedProducto = await _productoRepository.GetByIdAsync(dto.Id);
        var productoDto = _mapper.Map<ProductoDto>(updatedProducto);
        
        return Result<ProductoDto>.Ok(productoDto, "Producto actualizado con éxito");
    }

    public async Task<Result> DeleteProducto(int id)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto == null)
        {
            return Result.Fail("Producto no encontrado");
        }

        var deleted = await _productoRepository.DeleteAsync(id);
        return deleted 
            ? Result.Ok("Producto eliminado con éxito") 
            : Result.Fail("Error al eliminar el producto");
    }

    public async Task<IEnumerable<ProductoDto>> GetProductosByCategoria(int categoriaId)
    {
        var productos = await _productoRepository.GetByCategoryAsync(categoriaId);
        return _mapper.Map<IEnumerable<ProductoDto>>(productos);
    }

    public async Task<Result> UpdateStock(int productoId, int quantity)
    {
        var updated = await _productoRepository.UpdateStockAsync(productoId, quantity);
        return updated 
            ? Result.Ok("Stock actualizado con éxito") 
            : Result.Fail("Error al actualizar el stock");
    }
}