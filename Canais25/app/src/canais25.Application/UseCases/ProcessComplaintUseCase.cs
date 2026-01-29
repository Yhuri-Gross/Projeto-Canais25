using Core.Domain.Entities;
using Core.Ports.In;
using Core.Ports.Out;

namespace Application.UseCases;

public interface IClassifier
{
    IEnumerable<string> Classify(string text);
}

public class ProcessComplaintUseCase : IProcessComplaintUseCase
{
    private readonly IComplaintRepository _repository;
    private readonly IClassifier _classifier;

    public ProcessComplaintUseCase(
        IComplaintRepository repository,
        IClassifier classifier)
    {
        _repository = repository;
        _classifier = classifier;
    }

    public async Task ExecuteAsync(string id, string description)
    {
        var complaint = new Complaint(id, description);

        var categories = _classifier.Classify(description);
        complaint.Classify(categories);

        await _repository.SaveAsync(complaint);
    }
}
