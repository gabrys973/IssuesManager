using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Web.Middlewares;

public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch(ExternalErrorException e)
        {
            _logger.LogError(e, e.ErrorMessage);

            await HandleExceptionAsync(context, e);
        }
        catch(Exception e)
        {
            _logger.LogError(e, e.Message);

            await HandleExceptionAsync(context, e);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problem = new ProblemDetails
        {
            Instance = $"{context.Request.Method} {context.Request.Path}",
            Detail = exception?.Message
        };

        switch(exception)
        {
            case ExternalErrorException externalErrorException:
                problem.Type = "External Server Error";
                problem.Title = $"{externalErrorException.ServiceName} Server Error";
                problem.Status = (int)HttpStatusCode.ServiceUnavailable;
                break;

            default:
                problem.Type = "Server Error";
                problem.Title = "Server Error";
                problem.Status = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var response = JsonConvert.SerializeObject(problem);
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(response);
    }
}