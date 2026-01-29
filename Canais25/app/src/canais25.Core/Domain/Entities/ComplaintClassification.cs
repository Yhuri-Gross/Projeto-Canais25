namespace Canais25.Core.Domain.Entities;

public class ComplaintClassification
{
    public string Category { get; }
    public int MatchCount { get; }

    public ComplaintClassification(string category, int matchCount)
    {
        Category = category;
        MatchCount = matchCount;
    }
}
