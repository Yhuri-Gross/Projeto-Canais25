using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using Canais25.Core.Ports.In;

namespace Canais25.Adapters.Inbound.Sqs;

public class SqsConsumer
{
    private readonly IAmazonSQS _sqs;
    private readonly IProcessDocumentCommand _useCase;
    private readonly string _queueUrl;

    public SqsConsumer(
        IAmazonSQS sqs,
        IProcessDocumentCommand useCase,
        IConfiguration config)
    {
        _sqs = sqs;
        _useCase = useCase;
        _queueUrl = config["Sqs:QueueUrl"]!;
    }

    public async Task PollAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 5,
                WaitTimeSeconds = 20
            }, stoppingToken);

            foreach (var message in response.Messages)
            {
                try
                {
                    var payload = JsonSerializer.Deserialize<SqsMessage>(message.Body);

                    if (payload != null)
                    {
                        await _useCase.ExecuteAsync(payload.Bucket, payload.Key);
                    }

                    await _sqs.DeleteMessageAsync(_queueUrl, message.ReceiptHandle);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                    // aqui você pode mandar para DLQ
                }
            }
        }
    }
}
