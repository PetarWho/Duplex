using Duplex.Core.Common;
using Duplex.Core.Contracts.Administration;
using Duplex.Core.Models.Administration.Rank;
using Duplex.Infrastructure.Data.Models;
using Duplex.Infrastructure.Data.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Xml.Linq;

namespace Duplex.Core.Services.Administration
{
    public class RankService : IRankService
    {
        private readonly IRepository repo;
        private readonly RoleManager<IdentityRole> roleManager;

        public RankService(IRepository _repo, RoleManager<IdentityRole> _roleManager)
        {
            repo = _repo;
            roleManager = _roleManager;
        }

        public async Task<IdentityResult> CreateRankAsync(string name)
        {
            return await roleManager.CreateAsync(new IdentityRole(name));
        }

        public async Task DeleteRankAsync(string rankId)
        {
            var rank = await GetRankAsync(rankId);

            if(rank == null)
            {
                throw new Exception("No such rank!");
            }

            await roleManager.DeleteAsync(rank);

            await repo.SaveChangesAsync();
        }

        public async Task EditRankAsync(EditRankModel model)
        {
            var rank = await GetRankAsync(model.Id);

            rank.Name = model.Name;
            rank.NormalizedName = model.Name.ToUpper();

            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<RankModel>> GetAllAsync()
        {
            return await roleManager.Roles.Select(r => new RankModel()
            {
                Id = r.Id,
                Name = r.Name,
                ConcurrencyStamp = r.ConcurrencyStamp
            }).ToListAsync();
        }

        public async Task<IdentityRole> GetRankAsync(string rankId)
        {
            return await roleManager.FindByIdAsync(rankId);
        }
    }
}
