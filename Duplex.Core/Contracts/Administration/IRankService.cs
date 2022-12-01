using Duplex.Core.Models.Administration.Rank;
using Duplex.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Duplex.Core.Contracts.Administration
{
    public interface IRankService
    {
        Task<IdentityResult> CreateRankAsync(string name);
        Task<IEnumerable<RankModel>> GetAllAsync();
        Task<IdentityRole> GetRankAsync(string rankId);
        Task EditRankAsync(EditRankModel model);
        Task DeleteRankAsync(string rankId);
    }
}
