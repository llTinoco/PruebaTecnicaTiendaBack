using AutoMapper;
using InventoryApp.Application.DTOs;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Usuario mappings
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();

        // Producto mappings
        CreateMap<Producto, ProductoDto>()
            .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Name : string.Empty))
            .ForMember(dest => dest.ProveedorNombre, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.Name : string.Empty));
        CreateMap<CreateProductoDto, Producto>();
        CreateMap<UpdateProductoDto, Producto>();

        // OrdenVenta mappings
        CreateMap<OrdenesVenta, OrdenesVentaDto>();
        CreateMap<DetallesOrdenesVenta, DetallesOrdenesVentaDto>();
        CreateMap<CreateOrdenVentaDto, OrdenesVenta>();
    }
}