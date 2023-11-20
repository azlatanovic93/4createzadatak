using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Authentication;


namespace FourCreate.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)GetErrorCode(exception);
            string result = JsonConvert.SerializeObject(new ErrorDeatils
            {
                Id = context.TraceIdentifier,
                ErrorMessage = exception.Message,
                ErrorType = "Failure"
            });

            return context.Response.WriteAsync(result);
        }

        private static HttpStatusCode GetErrorCode(Exception ex)
        {
            return ex switch
            {
                ValidationException _ => HttpStatusCode.BadRequest,
                ArgumentException _ => HttpStatusCode.BadRequest,
                AuthenticationException _ => HttpStatusCode.Forbidden,
                NotImplementedException _ => HttpStatusCode.NotImplemented,
                _ => HttpStatusCode.InternalServerError
            };
        }
    }

    public class ErrorDeatils
    {
        public string? Id { get; set; }
        public string? ErrorType { get; set; }
        public string? ErrorMessage { get; set; }
    }

}

