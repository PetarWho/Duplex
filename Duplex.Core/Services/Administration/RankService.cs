using Duplex.Core.Common;
using Duplex.Core.Contracts.Administration;
using Duplex.Core.Models.Administration.Rank;
using Duplex.Infrastructure.Data.Models;
using Duplex.Infrastructure.Data.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace Duplex.Core.Services.Administration
{
    public class RankService : IRankService
    {
        private readonly IRepository repo;
        private readonly RoleManager<ApplicationUser> roleManager;

        public RankService(IRepository _repo, RoleManager<ApplicationUser> _roleManager)
        {
            repo = _repo;
            roleManager = _roleManager;
        }

        public async Task AddRankAsync(AddRankModel model)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRankAsync(Guid rankId)
        {
            throw new NotImplementedException();
        }

        public Task EditRankAsync(EditRankModel model)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RankModel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Event> GetRankAsync(Guid rankId)
        {
            throw new NotImplementedException();
        }
    }
}
