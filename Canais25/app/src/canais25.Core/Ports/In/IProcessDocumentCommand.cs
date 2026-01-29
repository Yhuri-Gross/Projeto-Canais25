namespace Canais25.Core.Ports.In;

public interface IProcessDocumentCommand
{
    Task ExecuteAsync(string bucket, string key);
}
