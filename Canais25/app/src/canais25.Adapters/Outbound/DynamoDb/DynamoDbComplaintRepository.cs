using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Canais25.Core.Domain.Entities;
using Canais25.Core.Ports.Out;
using System.Text.Json;

namespace Canais25.Adapters.Outbound.DynamoDb;

public class DynamoDbComplaintRepository : IComplaintRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private const string TableName = "canais25-complaints";

    public DynamoDbComplaintRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task SaveAsync(ComplaintRecord record)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            ["PK"] = new AttributeValue { S = record.ComplaintId },
            ["DocumentKey"] = new AttributeValue { S = record.DocumentKey },
            ["Status"] = new AttributeValue { S = record.Status },
            ["CreatedAt"] = new AttributeValue { S = record.CreatedAt.ToString("O") },
            ["ExtractedText"] = new AttributeValue { S = record.ExtractedText },
            ["Classifications"] = new AttributeValue
            {
                S = JsonSerializer.Serialize(record.Classifications)
            }
        };

        await _dynamoDb.PutItemAsync(new PutItemRequest
        {
            TableName = TableName,
            Item = item
        });
    }
}
