namespace Canais25.Core.Domain.Entities;

public class ComplaintRecord
{
    public string ComplaintId { get; init; } = Guid.NewGuid().ToString();
    public string DocumentKey { get; init; } = string.Empty;
    public string ExtractedText { get; init; } = string.Empty;
    public List<ComplaintClassification> Classifications { get; init; } = [];
    public string Status { get; init; } = "RECEIVED";
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
