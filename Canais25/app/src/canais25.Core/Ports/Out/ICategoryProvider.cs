namespace Canais25.Core.Ports.Out;

public interface ICategoryProvider
{
    Dictionary<string, List<string>> GetCategories();
}
