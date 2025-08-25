using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.Mappings;
using InventoryApp.Application.Services;
using InventoryApp.Application.Validators;
using InventoryApp.Domain.Repositories;
using InventoryApp.Domain.Entities;
using InventoryApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PruebaTecnicaTiendaFullstack.Middlewares;
using PruebaTecnicaTiendaFullstack.Models; // Agregar usando para JwtSettings

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddControllers();  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory API", Version = "v1" });
    
    // Configuración para JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Agregar AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Validaciones
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductoDtoValidator>();

// Inyección de dependencias - Repositorios
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IProductoRepository, ProductoRepository>();
builder.Services.AddTransient<IOrdenVentaRepository, OrdenVentaRepository>();

// Inyección de dependencias - Servicios
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IProductoService, ProductoService>();
builder.Services.AddTransient<IOrdenVentaService, OrdenVentaService>();

// Configuración JWT
var jwtSection = builder.Configuration.GetSection("JWT");
builder.Services.Configure<JwtSettings>(jwtSection);

var jwtSettings = jwtSection.Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience
    };
    
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["AuthToken"];
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configuración de middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware personalizado para manejo de errores
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
