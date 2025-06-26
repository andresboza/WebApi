using Microsoft.Extensions.Options;
using WebApi.Settings;
using System.Net;

namespace WebApi.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _configuredKey;

        public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeySettings> options)
        {
            _next = next;
            _configuredKey = options.Value.Key;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedKey))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("API Key faltante.");
                    return;
                }

                if (!_configuredKey.Equals(extractedKey))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync("API Key inválida.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
