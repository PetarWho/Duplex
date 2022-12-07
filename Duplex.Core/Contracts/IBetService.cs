using Duplex.Core.Models.Event;
using Duplex.Infrastructure.Data.Models;

namespace Duplex.Core.Contracts
{
    public interface IBetService
    {
        Task CreateBetAsync(Guid eventId, string userId);
        Task<IEnumerable<Bet>> GetAllAsync();
        Task<Bet> GetByEventAsync(Guid eventId);
        Task DeleteBetAsync(string userId, Guid eventId);
        Task<bool> ExistsByEvent(Guid eventId);
    }
}
