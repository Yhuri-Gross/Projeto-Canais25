namespace Canais25.Core.Ports.Out;

public interface IDocumentTextExtractor
{
    Task<string> ExtractTextAsync(string bucketName, string objectKey);
}
