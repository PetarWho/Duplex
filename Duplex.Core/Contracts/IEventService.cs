using Duplex.Core.Models.Event;

namespace Duplex.Core.Contracts
{
    public interface IEventService
    {
        Task AddEventAsync(AddEventModel model);
        Task<IEnumerable<EventModel>> GetAllAsync();
        Task<EditEventModel> GetEventAsync(Guid eId);
        Task<DetailsEventModel> GetEventWithParticipantsAsync(Guid eId);
        Task EditEventAsync(EditEventModel model);
        Task DeleteEventAsync(Guid eId);
        Task<bool> Exists(Guid eId);
        Task<IEnumerable<EventModel>> GetLastThree();
        Task JoinEvent(Guid eventId, string userId);
        Task LeaveEvent(Guid eventId, string userId);
    }
}
