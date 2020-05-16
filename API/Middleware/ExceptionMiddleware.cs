using System.Runtime.Serialization.Json;
using System.Net;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using API.Errors;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next,
                                    ILogger<ExceptionMiddleware> logger,
                                    IHostEnvironment env)
        {
            this.logger = logger;
            this.env = env;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            
                var response = env.IsDevelopment()
                ? new ApiException((int)HttpStatusCode.InternalServerError, ex.Message,
                            ex.StackTrace.ToString())
                : new ApiException((int)HttpStatusCode.InternalServerError);
            
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}