using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Adapters.Outbound.DynamoDb;
using Core.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Adapters.Tests.Outbound.DynamoDb
{
    public class DynamoComplaintRepositoryTests
    {
        [Fact]
        public async Task SaveAsync_Deve_Salvar_Complaint_No_DynamoDB()
        {
            var dynamoMock = new Mock<IAmazonDynamoDB>();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "DynamoDb:TableName", "ComplaintsTable" }
                })
                .Build();

            var repository = new DynamoComplaintRepository(
                dynamoMock.Object,
                configuration);

            var complaint = new Complaint
            {
                Id = "123",
                Description = "Cobrança indevida",
                CreatedAt = new DateTime(2024, 1, 1),
                Categories = new List<string> { "cobrança", "fraude" }
            };

            dynamoMock
                .Setup(d => d.PutItemAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, AttributeValue>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PutItemResponse());

            await repository.SaveAsync(complaint);

            dynamoMock.Verify(d => d.PutItemAsync(
                "ComplaintsTable",
                It.Is<Dictionary<string, AttributeValue>>(item =>
                    item["id"].S == "123" &&
                    item["description"].S == "Cobrança indevida" &&
                    item["createdAt"].S == complaint.CreatedAt.ToString("O") &&
                    item["categories"].SS.Contains("cobrança") &&
                    item["categories"].SS.Contains("fraude")
                ),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
