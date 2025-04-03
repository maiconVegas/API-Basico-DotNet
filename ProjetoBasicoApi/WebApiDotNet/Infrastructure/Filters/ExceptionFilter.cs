using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiDotNet.Infrastructure.Filters;

public class ExceptionFilter(IHostEnvironment environment, ILogger<ExceptionFilter> logger) : ExceptionFilterAttribute
{
    private readonly IHostEnvironment _environment = environment;
    private readonly ILogger<ExceptionFilter> _logger = logger;

    public override void OnException(ExceptionContext context)
    {
        var ex = context.Exception;
        _logger.LogCritical(ex, ex.Message);

        var errorModel = new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", new[] { "Houve um erro inesperado. Tente novamente mais tarde" } }
            })
        {
            Title = "Erro interno do servidor"
        };

        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Result = new JsonResult(errorModel);
    }
}
