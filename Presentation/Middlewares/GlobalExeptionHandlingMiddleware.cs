using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Presentation.Middlewears;

public class GlobalExeptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExeptionHandlingMiddleware> _logger;

    public GlobalExeptionHandlingMiddleware(ILogger<GlobalExeptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ArgumentNullException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Invalid Argument");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Argument Out of Range");
        }
        catch (ArgumentException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Invalid Argument Provided");
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.Unauthorized, "Unauthorized Access");
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Invalid Operation");
        }
        catch (NullReferenceException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "Null Reference Occurred");
        }
        catch (IndexOutOfRangeException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Index Out of Range");
        }
        catch (KeyNotFoundException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound, "Key Not Found");
        }
        catch (NotImplementedException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.NotImplemented, "Feature Not Implemented");
        }
        catch (TimeoutException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.RequestTimeout, "Operation Timed Out");
        }
        catch (IOException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "IO Exception Occurred");
        }
        catch (SqlException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "Database Error Occurred");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await WriteProblemDetailsAsync(context, HttpStatusCode.InternalServerError, "Server error", ex.Message);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode, string title)
    {
        _logger.LogError(ex, ex.Message);
        await WriteProblemDetailsAsync(context, statusCode, title, ex.ToString());
    }

    private async Task WriteProblemDetailsAsync(HttpContext context, HttpStatusCode statusCode, string title, string detail)
    {
        context.Response.StatusCode = (int)statusCode;
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Type = title
        };
        var json = JsonSerializer.Serialize(problemDetails);
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(json);
    }
}