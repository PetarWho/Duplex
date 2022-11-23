using Duplex.Core.Models.Administration.Rank;
using Duplex.Infrastructure.Data.Models;

namespace Duplex.Core.Contracts.Administration
{
    public interface IRankService
    {
        Task AddRankAsync(AddRankModel model);
        Task<IEnumerable<RankModel>> GetAllAsync();
        Task<Event> GetRankAsync(Guid rankId);
        Task EditRankAsync(EditRankModel model);
        Task DeleteRankAsync(Guid rankId);
    }
}
