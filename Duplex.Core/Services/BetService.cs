using Duplex.Core.Common;
using Duplex.Core.Contracts;
using Duplex.Core.Models.Event;
using Duplex.Infrastructure.Data.Models;
using Duplex.Infrastructure.Data.Models.Account;
using Microsoft.EntityFrameworkCore;

namespace Duplex.Core.Services
{
    public class BetService : IBetService
    {
        private readonly IRepository repo;

        public BetService(IRepository _repo)
        {
            repo= _repo;
        }
        public async Task CreateBetAsync(Guid eventId, string userId)
        {
            var ev = await repo.GetByIdAsync<Event>(eventId);
            var user = await repo.GetByIdAsync<ApplicationUser>(userId);

            var bet = await repo.AddAndReturnAsync<Bet>(new Bet()
            {
                EventId = eventId,
                CreatedOnUTC = DateTime.UtcNow,
                Event = ev,
                PrizePool = ev.EntryCost
            });

            await repo.AddAsync<UserBet>(new UserBet()
            {
                UserId = userId,
                User = user,
                BetId = bet.Id,
                Bet = bet
            });
        }

        public async Task DeleteBetAsync(string userId, Guid eventId)
        {
            var bet = await GetByEventAsync(eventId);

            var userBet = bet.UsersBets.First(x=>x.UserId == userId && x.Bet == bet);

            await repo.DeleteAsync<UserBet>(userBet);
        }

        public async Task<bool> ExistsByEvent(Guid eventId)
        {
            return await repo.AllReadonly<Bet>().AnyAsync(b=>b.EventId == eventId);
        }

        public async Task<IEnumerable<Bet>> GetAllAsync()
        {
            return await repo.AllReadonly<Bet>().ToListAsync();
        }

        public async Task<Bet> GetByEventAsync(Guid eventId)
        {
            return await repo.AllReadonly<Bet>().Include(x=>x.Event).FirstAsync(x => x.EventId == eventId);
        }
    }
}
