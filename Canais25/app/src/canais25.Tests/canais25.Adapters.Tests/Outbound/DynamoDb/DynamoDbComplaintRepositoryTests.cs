using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Canais25.Adapters.Outbound.DynamoDb;
using Canais25.Core.Domain.Entities;
using Moq;
using System.Text.Json;
using Xunit;

namespace Canais25.Tests.Adapters.Outbound.DynamoDb
{
    public class DynamoDbComplaintRepositoryTests
    {
        [Fact]
        public async Task SaveAsync_Deve_Persistir_ComplaintRecord_No_DynamoDb()
        {
            // Arrange
            var dynamoDbMock = new Mock<IAmazonDynamoDB>();

            var repository = new DynamoDbComplaintRepository(dynamoDbMock.Object);

            var record = new ComplaintRecord
            {
                ComplaintId = "cmp-123",
                DocumentKey = "doc-456",
                Status = "PROCESSED",
                CreatedAt = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                ExtractedText = "Texto extraído do documento",
                Classifications = new List<string> { "fraude", "cobrança" }
            };

            dynamoDbMock
                .Setup(d => d.PutItemAsync(
                    It.IsAny<PutItemRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PutItemResponse());

            // Act
            await repository.SaveAsync(record);

            // Assert
            dynamoDbMock.Verify(d => d.PutItemAsync(
                It.Is<PutItemRequest>(req =>
                    req.TableName == "canais25-complaints" &&
                    req.Item["PK"].S == "cmp-123" &&
                    req.Item["DocumentKey"].S == "doc-456" &&
                    req.Item["Status"].S == "PROCESSED" &&
                    req.Item["CreatedAt"].S == record.CreatedAt.ToString("O") &&
                    req.Item["ExtractedText"].S == "Texto extraído do documento" &&
                    JsonSerializer.Deserialize<List<string>>(req.Item["Classifications"].S!)!
                        .Contains("fraude") &&
                    JsonSerializer.Deserialize<List<string>>(req.Item["Classifications"].S!)!
                        .Contains("cobrança")
                ),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
