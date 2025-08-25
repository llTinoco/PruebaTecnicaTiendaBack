using System.Net;
using System.Text.Json;

namespace PruebaTecnicaTiendaFullstack.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = error switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                ApplicationException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError,
            };

            _logger.LogError(error, "Error: {Message}", error.Message);

            var result = new { message = error.Message };
            await response.WriteAsync(JsonSerializer.Serialize(result));
        }
    }
}