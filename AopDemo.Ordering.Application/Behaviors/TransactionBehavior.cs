using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AopDemo.Ordering.Application.Extensions;
using AopDemo.Ordering.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AopDemo.Ordering.Infrastructure;

namespace AopDemo.Ordering.Application.Behaviors
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly OrderingContext _orderingContext;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public TransactionBehaviour(OrderingContext orderingContext,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            _orderingContext = orderingContext ?? throw new ArgumentException(nameof(DbContext));
            _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentException(nameof(orderingIntegrationEventService));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
        }

        // Network resilience / managing optimistic concurrency
        // Handling many database operations (save changes) in an atomic manner
        // Publishes integration events
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                var strategy = _orderingContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    using (var transaction = await _orderingContext.Database.BeginTransactionAsync(cancellationToken))
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {RequestName} ({@Request})", transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {RequestName}", transaction.TransactionId, typeName);

                        try
                        {
                            await _orderingContext.SaveChangesAsync(cancellationToken);
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }

                        transactionId = transaction.TransactionId;
                    }

                    await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync(transactionId);
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {RequestName} ({@Request})", typeName, request);

                throw;
            }
        }
    }
}
