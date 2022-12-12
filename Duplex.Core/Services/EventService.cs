using Duplex.Core.Common;
using Duplex.Core.Contracts;
using Duplex.Core.Models.Event;
using Duplex.Infrastructure.Data.Models;
using Duplex.Infrastructure.Data.Models.Account;
using Microsoft.EntityFrameworkCore;

namespace Duplex.Core.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository repo;
        private readonly IBetService betService;

        public EventService(IRepository _repo, IBetService _betService)
        {
            repo = _repo;
            betService = _betService;
        }

        public async Task AddEventAsync(AddEventModel model)
        {
            await repo.AddAsync<Event>(new Event()
            {
                Name = model.Name,
                EntryCost = model.EntryCost,
                Description = model.Description,
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
            };
        }

        public async Task<EventModel> GetEventWithParticipantsAsync(Guid eId, string userId)
        {
            var model = await repo.AllReadonly<Event>().Include(x => x.Participants)
                .Select(x => new EventModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedOnUTC = x.CreatedOnUTC,
                    Description = x.Description,
                    EntryCost = x.EntryCost,
                    ImageUrl = x.ImageUrl,
                    Participants = x.Participants
                }).FirstOrDefaultAsync(x => x.Id == eId) ?? throw new ArgumentException("Null model");

            var eventUser = await repo.AllReadonly<EventUser>()
                .FirstOrDefaultAsync(x => x.EventId == eId && x.UserId == userId);

            if (eventUser == null)
            {
                model.IsDone = false;
            }
            else
            {
                model.IsDone = eventUser?.IsDone ?? throw new ArgumentException("Event-User Not found!");
            }

            return model;
        }

        public async Task<IEnumerable<EventModel>> GetLastThree()
        {
            return await repo.AllReadonly<Event>().Include(x => x.Participants).Select(x => new EventModel()
            {
                Id = x.Id,
                Name = x.Name,
                CreatedOnUTC = x.CreatedOnUTC,
                Description = x.Description,
                EntryCost = x.EntryCost,
                ImageUrl = x.ImageUrl,
                Participants = x.Participants
            })
                .OrderByDescending(x => x.CreatedOnUTC).Take(3).ToListAsync();
        }

        public async Task JoinEvent(Guid eventId, string userId)
        {
            var user = await repo.GetByIdAsync<ApplicationUser>(userId);

            await betService.CreateBetAsync(eventId, userId);

            var ev = await repo.GetByIdAsync<Event>(eventId);
            await repo.AddAsync<EventUser>(new EventUser()
            {
                ApplicationUser = user,
                UserId = userId,
                EventId = eventId,
                Event = ev
            });

            user.Coins -= ev.EntryCost;

            await repo.SaveChangesAsync();
        }

        public async Task LeaveEvent(Guid eventId, string userId)
        {
            var user = await repo.AllReadonly<ApplicationUser>().Include(x => x.Events).FirstOrDefaultAsync(x => x.Id == userId);
            var ev = await repo.AllReadonly<Event>().FirstAsync(x => x.Id == eventId);

            if (user != null)
            {
                var eventUser = user.Events.First(x => x.EventId == eventId);
                var bet = await betService.GetByEventAsync(eventId);
                //var userBet = await repo.AllReadonly<UserBet>().FirstAsync(x => x.UserId == userId && x.Bet == bet);

                bet.PrizePool -= ev.EntryCost;
                await repo.DeleteAsync<EventUser>(new { eventUser.UserId, eventUser.EventId });
                await repo.DeleteAsync<Bet>(bet.Id);

                await repo.SaveChangesAsync();
            }

        }
        public async Task VerifyDone(Guid eventId, string userId)
        {
            var user = await repo.All<ApplicationUser>().Include(x => x.Events).FirstOrDefaultAsync(x => x.Id == userId);
            var ev = await repo.GetByIdAsync<Event>(eventId);

            if (user == null)
            {
                throw new ArgumentException("No such user");
            }

            var userEvent = await repo.All<EventUser>().FirstAsync(x => x.EventId == ev.Id && x.UserId == userId);
            userEvent.IsDone = true;
            user.Coins += ev.EntryCost;

            await repo.SaveChangesAsync();
        }
    }
}
