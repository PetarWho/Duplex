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
            if (regId == 1)
            {
                throw new ArgumentException("Cannot delete that region!");
            }

            await repo.DeleteAsync<Region>(regId);

            await repo.SaveChangesAsync();
        }

        public async Task<RegionModel?> GetUnknownRegionAsync()
        {
            return await repo.AllReadonly<Region>().Select(x=> new RegionModel()
            {
                Id = x.Id,
                Code =x.Code,
                Name = x.Name
            }).FirstOrDefaultAsync(x => x.Name == "Unknown");
        }

        public async Task EditRegionAsync(RegionModel model)
        {
            var region = await repo.GetByIdAsync<Region>(model.Id);

            region.Code = model.Code;
            region.Name = model.Name;

            await repo.SaveChangesAsync();
        }

        public async Task<bool> Exists(int regId)
        {
            return await repo.AllReadonly<Region>().AnyAsync(e => e.Id == regId);
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

        public async Task<RegionModel> GetRegionAsync(int regId)
        {
            var reg = await repo.GetByIdAsync<Region>(regId);

            return new RegionModel()
            {
                Id = reg.Id,
                Name = reg.Name,
                Code = reg.Code
            };
        }
    }
}
