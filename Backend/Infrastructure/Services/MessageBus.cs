
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
    public async Task PublishAsync<T>(T command) where T : class
    {
        Console.WriteLine("hello");
        await Task.Delay(100);
    }

    public async Task SendAsync<TCommand>(TCommand command) where TCommand : IRequest
    {
        await _mediator.Send(command);
    }

    public async Task<TResponse> SendAsync<TCommand, TResponse>(TCommand command)
        where TCommand : IRequest<TResponse>
        where TResponse : class
    {
        return await _mediator.Send(command);
    }
}
