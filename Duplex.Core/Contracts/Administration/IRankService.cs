using Duplex.Core.Models.Administration.Rank;
using Duplex.Infrastructure.Data.Models;

namespace Duplex.Core.Contracts.Administration
{
    public interface IRankService
    {
        Task AddEventAsync(AddRankModel model);
        Task<IEnumerable<RankModel>> GetAllAsync();
        Task<Event> GetEventAsync(Guid rankId);
        Task EditEventAsync(EditRankModel model);
        Task DeleteEventAsync(Guid rankId);
    }
}
