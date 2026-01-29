using Canais25.Core.Ports.Out;

namespace Canais25.Adapters.Outbound.Classification;

public class InMemoryCategoryProvider : ICategoryProvider
{
    public Dictionary<string, List<string>> GetCategories()
    {
        return new()
        {
            ["imobiliario"] = new() { "credito imobiliario", "casa", "apartamento" },
            ["seguros"]     = new() { "resgate", "capitalizacao", "socorro" },
            ["cobranca"]    = new() { "fatura", "cobranca", "valor", "indevido" },
            ["acesso"]      = new() { "acessar", "login", "senha" },
            ["aplicativo"]  = new() { "app", "aplicativo", "travando", "erro" },
            ["fraude"]      = new() { "nao reconhece divida", "fraude" }
        };
    }
}
