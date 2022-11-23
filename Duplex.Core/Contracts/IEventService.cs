using Duplex.Core.Models.Event;
using Duplex.Core.Models.Prize;
using Duplex.Infrastructure.Data.Models;

namespace Duplex.Core.Contracts
{
    public interface IEventService
    {
        Task AddEventAsync(AddEventModel model);
        Task<IEnumerable<EventModel>> GetAllAsync();
        Task<Event> GetEventAsync(Guid eId);
        Task EditEventAsync(EditEventModel model);
        Task DeleteEventAsync(Guid eId);
        Task<bool> Exists(Guid eId);
    }
}
