using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PruebaTecnicaTiendaFullstack.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAll()
    {
        var productos = await _productoService.GetAllProductos();
        return Ok(productos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductoDto>> Get(int id)
    {
        var producto = await _productoService.GetProductoById(id);
        
        if (producto == null)
            return NotFound();
            
        return Ok(producto);
    }

    [HttpGet("categoria/{categoriaId}")]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> GetByCategoria(int categoriaId)
    {
        var productos = await _productoService.GetProductosByCategoria(categoriaId);
        return Ok(productos);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductoDto dto)
    {
        var result = await _productoService.CreateProducto(dto);
        
        if (!result.Success)
            return BadRequest(result);
            
        return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result.Data);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProductoDto dto)
    {
        var result = await _productoService.UpdateProducto(dto);
        
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productoService.DeleteProducto(id);
        
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] StockUpdateRequest request)
    {
        var result = await _productoService.UpdateStock(id, request.Quantity);
        
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }
}

public class StockUpdateRequest
{
    public int Quantity { get; set; }
}