using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using Newtonsoft.Json;
using RepositoryUOWDomain.Shared.Extensions;
using RepositoryUOWDomain.ValueObject;

namespace RepositoryUOWInfrastructure.Middleware;

public class GlobalExceptionHandler : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandler>? _logger;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler>? logger)
    {
        _logger = logger;
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger<GlobalExceptionHandler>? logger)
    {
        string errorMessage = await ex.ToErrorStr() ?? "";

        logger?.LogError($"Exception details: {errorMessage}" );

        var result = JsonConvert.SerializeObject(new ExceptionDetail { StatusCoDe = HttpStatusCode.InternalServerError, Message = errorMessage });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(result).ConfigureAwait(false);
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var F = await new StreamReader(context.Response.Body).ReadToEndAsync();
            _logger?.Log (LogLevel.Information,$"{await new StreamReader(context.Response.Body).ReadToEndAsync()}");
            await next(context);
        }
        catch (Exception exceptionObj)
        {
            await HandleExceptionAsync(context, exceptionObj, _logger);
        }
    }
}