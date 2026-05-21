using MediatR;

namespace SupportHub.Application.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
    
}