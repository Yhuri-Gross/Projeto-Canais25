namespace Core.Ports.In;

public interface IProcessComplaintUseCase
{
    Task ExecuteAsync(string id, string description);
}