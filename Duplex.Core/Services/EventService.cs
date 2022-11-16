using Duplex.Core.Common;
using Duplex.Core.Contracts;
using Duplex.Core.Models.Event;
using Duplex.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Duplex.Core.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository repo;

        public EventService(IRepository _repo)
        {
            repo = _repo;
        }

        public async Task AddEventAsync(AddEventModel model)
        {
            await repo.AddAsync<Event>(new Event()
            {
                Name = model.Name,
                EntryCost = model.EntryCost,
                Description = model.Description,
                TeamSize = model.TeamSize,
                ImageUrl = model.ImageUrl,
                CreatedOnUTC = DateTime.UtcNow
            });

            await repo.SaveChangesAsync();
        }

        public async Task DeleteEventAsync(Guid eId)
        {
            await repo.DeleteAsync<Event>(eId);
            await repo.SaveChangesAsync();
        }

        public async Task EditEventAsync(EditEventModel model)
        {
            var ev = await repo.GetByIdAsync<Event>(model.Id);

            ev.Name = model.Name;
            ev.Description = model.Description;
            ev.EntryCost = model.EntryCost;
            ev.TeamSize = model.TeamSize;
            ev.ImageUrl = model.ImageUrl;
            ev.CreatedOnUTC = DateTime.UtcNow;

            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<EventModel>> GetAllAsync()
        {
            return await repo.AllReadonly<Event>().Select(e => new EventModel()
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                EntryCost = e.EntryCost,
                ImageUrl = e.ImageUrl,
                TeamSize = e.TeamSize,
                CreatedOnUTC = e.CreatedOnUTC
            }).ToListAsync();
        }

        public async Task<Event> GetEventAsync(Guid eId)
        {
            return await repo.GetByIdAsync<Event>(eId);
        }
    }
}
