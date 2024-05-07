using WebGallery.Api.Helpers;

namespace WebGallery.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context, IErrorResponseFormatter formatter)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exc)
        {
            var (statusCode, response) = formatter.GetErrorResponse(exc);

            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
