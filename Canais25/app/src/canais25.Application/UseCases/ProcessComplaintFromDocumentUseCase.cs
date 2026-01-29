using Canais25.Core.Ports.Out;

namespace Canais25.Application.UseCases;

public class ProcessComplaintFromDocumentUseCase
{
    private readonly IDocumentTextExtractor _extractor;

    public ProcessComplaintFromDocumentUseCase(
        IDocumentTextExtractor extractor)
    {
        _extractor = extractor;
    }

    public async Task<string> ExecuteAsync(string bucket, string key)
    {
        var text = await _extractor.ExtractTextAsync(bucket, key);

        return text;
    }
}
