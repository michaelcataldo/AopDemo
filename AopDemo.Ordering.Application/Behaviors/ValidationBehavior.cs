using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AopDemo.Ordering.Domain.Exceptions;
using AopDemo.Ordering.Application.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AopDemo.Ordering.Application.Behaviors
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
        private readonly IValidator<TRequest>[] _validators;

        public ValidatorBehavior(IValidator<TRequest>[] validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var typeName = request.GetGenericTypeName();

            _logger.LogInformation("----- Validating request {RequestType}", typeName);

            var failures = _validators.Select(v => v.Validate(request)).SelectMany(r => r.Errors).Where(e => e != null).ToList();

            if (!failures.Any())
            {
                return await next();
            }

            _logger.LogWarning("Validation errors - {RequestType} - Request: {@Request} - Errors: {@ValidationErrors}", typeName, request, failures);

            throw new OrderingDomainException(
                $"Request validation errors for type {typeof(TRequest).Name}", new ValidationException(failures));
        }
    }
}
