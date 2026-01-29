using Amazon.SQS;
using Amazon.SQS.Model;
using Core.Ports.In;
using Microsoft.Extensions.Hosting;

namespace Adapters.Inbound.Sqs;

public class SqsWorker : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly IProcessComplaintUseCase _useCase;
    private readonly string _queueUrl;

    public SqsWorker(
        IAmazonSQS sqs,
        IProcessComplaintUseCase useCase,
        IConfiguration config)
    {
        _sqs = sqs;
        _useCase = useCase;
        _queueUrl = config["Sqs:QueueUrl"]!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20
            }, stoppingToken);

            foreach (var message in response.Messages)
            {
                var body = System.Text.Json.JsonDocument.Parse(message.Body);

                await _useCase.ExecuteAsync(
                    body.RootElement.GetProperty("id").GetString()!,
                    body.RootElement.GetProperty("description").GetString()!
                );

                await _sqs.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, stoppingToken);
            }
        }
    }
}
