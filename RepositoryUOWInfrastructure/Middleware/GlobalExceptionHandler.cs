using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using Newtonsoft.Json;
using RepositoryUOWDomain.Shared.Extensions;
using System.Text;

namespace RepositoryUOWInfrastructure.Middleware;

public class GlobalExceptionHandler : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandler>? _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler>? logger)
    {
        _logger = logger;
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex,
        ILogger<GlobalExceptionHandler>? logger)
    {
        string errorMessage = await ex.ToErrorStr() ?? "";
        logger?.LogError($"{nameof(Exception)} details: {errorMessage}");
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = errorMessage
        })).ConfigureAwait(false);
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            string requestStr = await FormatRequest(context.Request);
            using var responseBody = new MemoryStream();
            Stream originalBodyStream = context.Response.Body;
            context.Response.Body = responseBody;
            await next(context);
            _logger?.Log(LogLevel.Information,
                $"Requested - {requestStr} {Environment.NewLine} Response - {await FormatResponse(context.Response)}");
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception exceptionObj)
        {
            await HandleExceptionAsync(context, exceptionObj, _logger);
        }
    }


    private async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        string text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return $"{response.StatusCode}: {text}";
    }


    private async Task<string> FormatRequest(HttpRequest request)
    {
        var body = request.Body;
        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        int _ = await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        request.Body = body;
        return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
    }
}