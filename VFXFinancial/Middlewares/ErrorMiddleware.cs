using System.Net;
using Core.Exceptions;
using FluentValidation;
using Newtonsoft.Json;

namespace VFXFinancial.Middlewares;

public class ErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorMiddleware> _logger;

    public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
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
        // Fluent validation exceptions
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            context.Response.ContentType = "application/json";

            var errors = ex.Errors.Select(error => error.ErrorMessage).ToHashSet();
            var errorResponse = JsonConvert.SerializeObject(new { errors });
            await context.Response.WriteAsync(errorResponse);

            _logger.LogError(ex, "[ErrorMiddleware] - An error was caught");
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            context.Response.ContentType = "application/json";
            var errorResponse = JsonConvert.SerializeObject(new { errors = ex.Message });
            await context.Response.WriteAsync(errorResponse);

            _logger.LogError(ex, "[ErrorMiddleware] - An error was caught");
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";

            var errorResponse = JsonConvert.SerializeObject(new { errors = ex.Message });

            await context.Response.WriteAsync(errorResponse);

            _logger.LogError(ex, "[ErrorMiddleware] - An error was caught");
        }
    }
}