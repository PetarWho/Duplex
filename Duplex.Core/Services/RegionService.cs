using Duplex.Core.Common;
using Duplex.Core.Contracts;
using Duplex.Core.Models;
using Duplex.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Duplex.Core.Services
{
    public class RegionService : IRegionService
    {
        private readonly IRepository repo;

        public RegionService(IRepository _repo)
        {
            repo = _repo;
        }

        public async Task AddRegionAsync(RegionModel model)
        {
            var region = new Region()
            {
                Name = model.Name,
                Code = model.Code
            };

            await repo.AddAsync(region);
            await repo.SaveChangesAsync();
        }

        public async Task DeleteRegionAsync(int regId)
        {
            await repo.DeleteAsync<Region>(regId);

            await repo.SaveChangesAsync();
        }

        public async Task EditRegionAsync(RegionModel model)
        {
            var region = await repo.GetByIdAsync<Region>(model.Id);

            region.Code = model.Code;
            region.Name = model.Name;

            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<RegionModel>> GetAllAsync()
        {
            var regions = await repo.AllReadonly<Region>().ToListAsync();

            return regions.Select(r => new RegionModel()
            {
                Id = r.Id,
                Name = r.Name,
                Code = r.Code
            });
        }

        public async Task<Region> GetRegionAsync(int regId)
        {
            return await repo.GetByIdAsync<Region>(regId);
        }
    }
}
