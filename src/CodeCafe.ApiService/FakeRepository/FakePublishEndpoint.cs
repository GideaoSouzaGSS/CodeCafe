// Fake para IPublishEndpoint
using MassTransit;

public class FakePublishEndpoint : MassTransit.IPublishEndpoint
{
    public Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class => Task.CompletedTask;
    public Task Publish<T>(T message, IPipe<MassTransit.PublishContext<T>> publishPipe, CancellationToken cancellationToken = default) where T : class => Task.CompletedTask;
    public Task Publish(object message, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Publish(object message, Type messageType, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Publish(object message, MassTransit.IPipe<MassTransit.PublishContext> publishPipe, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Publish(object message, Type messageType, MassTransit.IPipe<MassTransit.PublishContext> publishPipe, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public MassTransit.ConnectHandle ConnectPublishObserver(MassTransit.IPublishObserver observer) => null!;



    public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task Publish<T>(object values, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }
}