using Amazon.SQS;
using Amazon.SQS.Model;
using Adapters.Inbound.Sqs;
using Core.Ports.In;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text.Json;
using Xunit;

namespace Adapters.Tests.Inbound.Sqs
{
    public class SqsWorkerTests
    {
        private class TestableSqsWorker : SqsWorker
        {
            public TestableSqsWorker(
                IAmazonSQS sqs,
                IProcessComplaintUseCase useCase,
                IConfiguration config)
                : base(sqs, useCase, config)
            {
            }

            public Task RunAsync(CancellationToken token)
                => base.ExecuteAsync(token);
        }

        [Fact]
        public async Task ExecuteAsync_Deve_Processar_Mensagem_E_Deletar()
        {
            var sqsMock = new Mock<IAmazonSQS>();
            var useCaseMock = new Mock<IProcessComplaintUseCase>();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Sqs:QueueUrl", "https://fake-queue-url" }
                })
                .Build();

            var messageBody = JsonSerializer.Serialize(new
            {
                id = "123",
                description = "teste"
            });

            var message = new Message
            {
                Body = messageBody,
                ReceiptHandle = "receipt-handle"
            };

            sqsMock
                .Setup(s => s.ReceiveMessageAsync(
                    It.IsAny<ReceiveMessageRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiveMessageResponse
                {
                    Messages = new List<Message> { message }
                });

            sqsMock
                .Setup(s => s.DeleteMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteMessageResponse());

            var worker = new TestableSqsWorker(
                sqsMock.Object,
                useCaseMock.Object,
                configuration);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            await worker.RunAsync(cts.Token);

            useCaseMock.Verify(
                u => u.ExecuteAsync("123", "teste"),
                Times.Once);

            sqsMock.Verify(
                s => s.DeleteMessageAsync(
                    "https://fake-queue-url",
                    "receipt-handle",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
