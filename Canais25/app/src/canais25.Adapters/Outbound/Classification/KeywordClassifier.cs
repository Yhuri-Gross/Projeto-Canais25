using Core.Ports.In;

namespace Core.Domain.Services;

public class KeywordClassifier : IClassifier
{
    private static readonly Dictionary<string, string[]> Rules = new()
    {
        ["fraude"] = ["fraude", "não reconheço", "não reconhece"],
        ["cobrança"] = ["cobrança", "valor indevido", "fatura"],
        ["acesso"] = ["login", "senha", "acesso"],
        ["app"] = ["aplicativo", "app", "erro"]
    };

    public IEnumerable<string> Classify(string text)
    {
        var normalized = text.ToLowerInvariant();

        return Rules
            .Where(r => r.Value.Any(k => normalized.Contains(k)))
            .Select(r => r.Key);
    }
}
