using Duplex.Core.Common;
using Duplex.Core.Contracts;
using Duplex.Core.Models.Event;
using Duplex.Data;
using Duplex.Infrastructure.Data.Models;
using Duplex.Infrastructure.Data.Models.Account;
using Microsoft.EntityFrameworkCore;

namespace Duplex.Core.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository repo;
        private readonly ApplicationDbContext context;

        public EventService(IRepository _repo, ApplicationDbContext _context)
        {
            repo = _repo;
            context = _context;
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

        public async Task<bool> Exists(Guid eId)
        {
            return await repo.AllReadonly<Event>().AnyAsync(e => e.Id == eId);
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
                CreatedOnUTC = e.CreatedOnUTC,
                Participants = e.Participants
            }).ToListAsync();
        }

        public async Task<EditEventModel> GetEventAsync(Guid eId)
        {
            var result = await repo.GetByIdAsync<Event>(eId);

            return new EditEventModel()
            {
                Id = result.Id,
                Name = result.Name,
                EntryCost = result.EntryCost,
                Description = result.Description,
                ImageUrl = result.ImageUrl,
                TeamSize = result.TeamSize,
            };
        }

        public async Task<EventModel> GetEventWithParticipantsAsync(Guid eId)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await context.Events.Select(x => new EventModel()
            {
                Id = x.Id,
                Name = x.Name,
                TeamSize = x.TeamSize,
                CreatedOnUTC = x.CreatedOnUTC,
                Description = x.Description,
                EntryCost = x.EntryCost,
                ImageUrl = x.ImageUrl,
                Participants = x.Participants
            }).FirstOrDefaultAsync(x => x.Id == eId);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<IEnumerable<EventModel>> GetLastThree()
        {
            return await repo.AllReadonly<Event>().Select(x => new EventModel()
            {
                Id = x.Id,
                Name = x.Name,
                CreatedOnUTC = x.CreatedOnUTC,
                Description = x.Description,
                EntryCost = x.EntryCost,
                TeamSize = x.TeamSize,
                ImageUrl = x.ImageUrl
            })
                .OrderByDescending(x => x.CreatedOnUTC).Take(3).ToListAsync();
        }

        public async Task JoinEvent(Guid eventId, string userId)
        {
            var user = await repo.GetByIdAsync<ApplicationUser>(userId);
            var ev = await repo.GetByIdAsync<Event>(eventId);

            var eventUser = new EventUser() { ApplicationUser=user, UserId = userId, EventId = eventId, Event = ev};
            await repo.AddAsync<EventUser>(eventUser);

            user.Coins -= ev.EntryCost;

            await repo.SaveChangesAsync();
        }

        public async Task LeaveEvent(Guid eventId, string userId)
        {
            var user = await repo.AllReadonly<ApplicationUser>().Include(x => x.Events).FirstOrDefaultAsync(x => x.Id == userId);

            if (user != null)
            {
                var eventUser = user.Events.First(x => x.EventId == eventId);

                context.EventsUsers.Remove(eventUser);

                await repo.SaveChangesAsync();
            }

        }
    }
}
