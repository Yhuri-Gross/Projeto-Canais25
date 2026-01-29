using Amazon.SQS;
using Amazon.SQS.Model;
using Canais25.Adapters.Inbound.Sqs;
using Canais25.Core.Ports.In;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text.Json;
using Xunit;

namespace Canais25.Tests.Adapters.Inbound.Sqs
{
    public class SqsConsumerTests
    {
        private readonly Mock<IAmazonSQS> _sqsMock;
        private readonly Mock<IProcessDocumentCommand> _useCaseMock;
        private readonly IConfiguration _configuration;
        private readonly SqsConsumer _consumer;

        public SqsConsumerTests()
        {
            _sqsMock = new Mock<IAmazonSQS>();
            _useCaseMock = new Mock<IProcessDocumentCommand>();

            var configValues = new Dictionary<string, string?>
            {
                { "Sqs:QueueUrl", "https://fake-queue-url" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            _consumer = new SqsConsumer(
                _sqsMock.Object,
                _useCaseMock.Object,
                _configuration);
        }

        [Fact]
        public async Task PollAsync_Deve_Processar_Mensagem_Valida_E_Deletar()
        {
            // Arrange
            var payload = new SqsMessage
            {
                Bucket = "bucket-test",
                Key = "file.pdf"
            };

            var message = new Message
            {
                Body = JsonSerializer.Serialize(payload),
                ReceiptHandle = "receipt-handle"
            };

            _sqsMock
                .Setup(s => s.ReceiveMessageAsync(
                    It.IsAny<ReceiveMessageRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiveMessageResponse
                {
                    Messages = new List<Message> { message }
                });

            _sqsMock
                .Setup(s => s.DeleteMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteMessageResponse());

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act
            await _consumer.PollAsync(cts.Token);

            // Assert
            _useCaseMock.Verify(
                u => u.ExecuteAsync("bucket-test", "file.pdf"),
                Times.Once);

            _sqsMock.Verify(
                s => s.DeleteMessageAsync(
                    "https://fake-queue-url",
                    "receipt-handle",
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PollAsync_Nao_Deve_Chamar_UseCase_Quando_Payload_For_Nulo()
        {
            // Arrange
            var message = new Message
            {
                Body = "null",
                ReceiptHandle = "receipt-handle"
            };

            _sqsMock
                .Setup(s => s.ReceiveMessageAsync(
                    It.IsAny<ReceiveMessageRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiveMessageResponse
                {
                    Messages = new List<Message> { message }
                });

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act
            await _consumer.PollAsync(cts.Token);

            // Assert
            _useCaseMock.Verify(
                u => u.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task PollAsync_Deve_Tratar_Excecao_Sem_Lancar()
        {
            // Arrange
            var payload = new SqsMessage
            {
                Bucket = "bucket",
                Key = "file"
            };

            var message = new Message
            {
                Body = JsonSerializer.Serialize(payload),
                ReceiptHandle = "receipt-handle"
            };

            _sqsMock
                .Setup(s => s.ReceiveMessageAsync(
                    It.IsAny<ReceiveMessageRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiveMessageResponse
                {
                    Messages = new List<Message> { message }
                });

            _useCaseMock
                .Setup(u => u.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act
            await _consumer.PollAsync(cts.Token);

            // Assert
            _useCaseMock.Verify(
                u => u.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }
    }
}
