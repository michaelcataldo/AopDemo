using System;
using System.Collections.Generic;
using System.Linq;
using AopDemo.Ordering.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace AopDemo.Ordering.Api.Setup.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);

            if (context.Exception is OrderingDomainException ex)
            {
                const int status = StatusCodes.Status400BadRequest;

                var errors = (ex.InnerException as ValidationException)?.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Select(e => e.ErrorMessage).ToArray());

                var problemDetails = new ValidationProblemDetails(errors ?? new Dictionary<string, string[]>())
                {
                    Status = status,
                    Detail = errors != null
                        ? "Please refer to the errors property for additional details."
                        : ex.Message,
                    Instance = context.HttpContext.Request.Path
                };

                context.Result = new BadRequestObjectResult(problemDetails);
                context.HttpContext.Response.StatusCode = status;
                context.ExceptionHandled = true;
            }
        }
    }
}
