using Canais25.Adapters.Inbound.Sqs;

namespace Canais25.Worker;

public class SqsWorker : BackgroundService
{
    private readonly SqsConsumer _consumer;

    public SqsWorker(SqsConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.PollAsync(stoppingToken);
    }
}
