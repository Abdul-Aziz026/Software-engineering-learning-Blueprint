using Application.Common.Interfaces.Publisher;
using MediatR;

namespace Infrastructure.Services;

public class MessageBus : IMessageBus
{
    private readonly IMediator _mediator;

    public MessageBus(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task SendAsync<TCommand>(TCommand command) where TCommand : IRequest
        => _mediator.Send(command);

    public Task<TResponse> SendAsync<TCommand, TResponse>(TCommand command)
        where TCommand : IRequest<TResponse>
        where TResponse : class
        => _mediator.Send(command);

    public Task PublishAsync<T>(T command) where T : class
        => throw new NotImplementedException(
            "PublishAsync is not yet implemented. Wire MassTransit or another broker before using this method.");
}
