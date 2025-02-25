using Infinity.Toolkit.Messaging.Abstractions;

namespace Infinity.Toolkit.Messaging;

internal sealed class MessageBus(IEnumerable<IBroker> brokers, ILogger<MessageBus> logger) : IMessageBus
{
    private readonly IEnumerable<IBroker> brokers = brokers;

    public ILogger<MessageBus> Logger { get; } = logger;

    public bool IsProcessing => brokers.Any(x => x.IsProcessing);

    public IEnumerable<IBroker> Brokers => brokers;

    public async Task InitAsync()
    {
        Logger?.InitializingMessageBus();
        foreach (var broker in brokers)
        {
            await broker.InitAsync();
        }
    }

    public async Task StartAsync(bool forceStart = false, CancellationToken cancellationToken = default)
    {
        Logger?.StartingMessageBus();
        foreach (var broker in brokers)
        {
            if (broker.AutoStartListening || forceStart)
            {
                await broker.StartAsync(cancellationToken);
            }
        }
    }

    public Task StartAsync(IBroker messageBroker, CancellationToken cancellationToken = default)
    {
        Logger?.StartingMessageBroker(messageBroker.Name);
        ArgumentNullException.ThrowIfNull(messageBroker);
        return messageBroker.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        Logger?.StoppingMessageBus();
        foreach (var broker in brokers)
        {
            await broker.StopAsync(cancellationToken);
        }
    }

    public Task StopAsync(IBroker messageBroker, CancellationToken cancellationToken = default)
    {
        Logger?.StoppingMessageBroker(messageBroker.Name);
        ArgumentNullException.ThrowIfNull(messageBroker);
        return messageBroker.StopAsync(cancellationToken);
    }
}
