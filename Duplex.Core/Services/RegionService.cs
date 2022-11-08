using Duplex.Core.Common;
using Duplex.Core.Contracts;
using Duplex.Core.Models;
using Duplex.Infrastructure.Data.Models;

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
    }
}
