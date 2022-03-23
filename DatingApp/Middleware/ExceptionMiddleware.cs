using DatingApp.Errors;
using System.Net;
using System.Text.Json;

namespace DatingApp.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";

                context.Response.StatusCode = ex switch
                {
                    InvalidActionException  => (int)HttpStatusCode.BadRequest,
                    UnauthorizedException   => (int)HttpStatusCode.Unauthorized,
                    NotFoundException       => (int)HttpStatusCode.NotFound,
                    _                       => (int)HttpStatusCode.InternalServerError,
                };
                ApiException response;
                if (_env.IsDevelopment() && context.Response.StatusCode == 500)
                {
                    response = new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString());
                } 
                else if (context.Response.StatusCode == (int)HttpStatusCode.InternalServerError)
                {
                    response = new ApiException(context.Response.StatusCode, "Internal Server Error");
                } 
                else
                {
                    response = new ApiException(context.Response.StatusCode, ex.Message);
                }

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
