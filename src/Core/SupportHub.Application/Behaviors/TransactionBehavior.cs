using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Messaging;
using SupportHub.Application.Abstractions.Transactions;

namespace SupportHub.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(
     IUnitOfWork unitOfWork,
     ILogger<TransactionBehavior<TRequest,TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
     where TRequest : ICommand<TResponse>
{
     public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
     {
          logger.LogInformation("Starting transaction for {RequestType}", typeof(TRequest).Name);
          await unitOfWork.BeginTransactionAsync(cancellationToken);
          try
          {
               var response = await next(cancellationToken);
               await unitOfWork.SaveChangesAsync(cancellationToken);
               await unitOfWork.CommitTransactionAsync(cancellationToken);
               logger.LogInformation("Transaction committed for {RequestType}", typeof(TRequest).Name);
               return response;
          }
          catch(Exception ex)
          {
               await unitOfWork.RollbackTransactionAsync(cancellationToken);
               logger.LogWarning("Transaction rolled back for {RequestType} due to exception: {ExceptionMessage}", typeof(TRequest).Name, ex.Message);
               throw;
          }
     }
}