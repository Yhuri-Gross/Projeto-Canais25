using Canais25.Core.Domain.Entities;

namespace Canais25.Core.Domain.Services;

public class KeywordClassifier
{
    public IReadOnlyList<ComplaintClassification> Classify(
        string text,
        Dictionary<string, List<string>> categories)
    {
        var normalizedText = text.ToLowerInvariant();
        var results = new List<ComplaintClassification>();

        foreach (var category in categories)
        {
            var count = category.Value.Count(keyword =>
                normalizedText.Contains(keyword.ToLowerInvariant()));

            if (count > 0)
            {
                results.Add(new ComplaintClassification(category.Key, count));
            }
        }

        return results
            .OrderByDescending(r => r.MatchCount)
            .ToList();
    }
}
