using Canais25.Core.Domain.Entities;
using Canais25.Core.Domain.Services;
using Canais25.Core.Ports.In;
using Canais25.Core.Ports.Out;

namespace Canais25.Application.UseCases;

public class ProcessDocumentUseCase : IProcessDocumentCommand
{
    private readonly IDocumentTextExtractor _textExtractor;
    private readonly ICategoryProvider _categoryProvider;
    private readonly IComplaintRepository _repository;
    private readonly KeywordClassifier _classifier;

    public ProcessDocumentUseCase(
        IDocumentTextExtractor textExtractor,
        ICategoryProvider categoryProvider,
        IComplaintRepository repository)
    {
        _textExtractor = textExtractor;
        _categoryProvider = categoryProvider;
        _repository = repository;
        _classifier = new KeywordClassifier();
    }

    public async Task ExecuteAsync(string bucket, string key)
    {
        var extractedText = await _textExtractor.ExtractTextAsync(bucket, key);

        var categories = _categoryProvider.GetCategories();
        var classifications = _classifier.Classify(extractedText, categories);

        var record = new ComplaintRecord
        {
            DocumentKey = key,
            ExtractedText = extractedText,
            Classifications = classifications.ToList(),
            Status = "CLASSIFIED"
        };

        await _repository.SaveAsync(record);

        Console.WriteLine($"Reclamação salva: {record.ComplaintId}");
    }
}
