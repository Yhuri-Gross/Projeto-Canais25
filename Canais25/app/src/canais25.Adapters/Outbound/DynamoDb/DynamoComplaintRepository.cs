using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Core.Domain.Entities;
using Core.Ports.Out;

namespace Adapters.Outbound.DynamoDb;

public class DynamoComplaintRepository : IComplaintRepository
{
    private readonly IAmazonDynamoDB _client;
    private readonly string _tableName;

    public DynamoComplaintRepository(
        IAmazonDynamoDB client,
        IConfiguration config)
    {
        _client = client;
        _tableName = config["DynamoDb:TableName"]!;
    }

    public async Task SaveAsync(Complaint complaint)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            ["id"] = new AttributeValue { S = complaint.Id },
            ["description"] = new AttributeValue { S = complaint.Description },
            ["createdAt"] = new AttributeValue { S = complaint.CreatedAt.ToString("O") },
            ["categories"] = new AttributeValue { SS = complaint.Categories }
        };

        await _client.PutItemAsync(_tableName, item);
    }
}
