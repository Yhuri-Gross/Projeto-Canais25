using Canais25.Core.Domain.Entities;

namespace Canais25.Core.Ports.Out;

public interface IComplaintRepository
{
    Task SaveAsync(ComplaintRecord record);
}
