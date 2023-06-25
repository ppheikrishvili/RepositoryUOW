namespace RepositoryUOWService.Model;

public class RequestResponseLoggingMiddleware : IMiddleware
{
    protected readonly RequestDelegate _next;
    protected readonly ILogger _logger;
    public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>(); 
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await _next(context);
    }
}