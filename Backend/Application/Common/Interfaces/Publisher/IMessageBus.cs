using MediatR;

namespace Application.Common.Interfaces.Publisher;

public interface IMessageBus
{
    Task SendAsync<TCommand>(TCommand command) where TCommand : IRequest;

    Task<TResponse> SendAsync<TCommand, TResponse>(TCommand command) where TCommand : IRequest<TResponse>
                                                                     where TResponse : class;
    Task PublishAsync<T>(T command) where T: class;
}
