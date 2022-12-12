using Duplex.Core.Models.Event;
using Duplex.Infrastructure.Data.Models.Account;

namespace Duplex.Core.Contracts
{
    public interface IEventService
    {
        Task AddEventAsync(AddEventModel model);
        Task<IEnumerable<EventModel>> GetAllAsync();
        Task<EditEventModel> GetEventAsync(Guid eId);
        Task<EventModel> GetEventWithParticipantsAsync(Guid eId, string userId);
        Task EditEventAsync(EditEventModel model);
        Task DeleteEventAsync(Guid eId);
        Task<bool> Exists(Guid eId);
        Task<IEnumerable<EventModel>> GetLastThree();
        Task JoinEvent(Guid eventId, string userId);
        Task LeaveEvent(Guid eventId, string userId);
        Task VerifyDone(Guid eventId, string userId);
    }
}
