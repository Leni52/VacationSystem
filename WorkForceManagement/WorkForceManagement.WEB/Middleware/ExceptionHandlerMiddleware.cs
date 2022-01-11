using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.BLL.Exceptions.WorkForceManagement.BLL;

namespace WorkForceManagement.WEB.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        public readonly RequestDelegate _next;
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
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

                switch (error)
                {
                    case ItemDoesNotExistException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case ItemAlreadyExistsException e:                       
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        break;
                    case InvalidIdException e:                       
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                       break;

                    default:                       
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);

            }
        }
    }
}