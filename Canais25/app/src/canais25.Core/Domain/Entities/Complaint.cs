namespace Core.Domain.Entities;

public class Complaint
{
    public string Id { get; }
    public string Description { get; }
    public List<string> Categories { get; private set; }
    public DateTime CreatedAt { get; }

    public Complaint(string id, string description)
    {
        Id = id;
        Description = description;
        Categories = new List<string>();
        CreatedAt = DateTime.UtcNow;
    }

    public void Classify(IEnumerable<string> categories)
    {
        Categories = categories.ToList();
    }
}
