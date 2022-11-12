using Duplex.Core.Models;
using Duplex.Infrastructure.Data.Models;

namespace Duplex.Core.Contracts
{
    public interface IRegionService
    {
        Task AddRegionAsync(RegionModel model);
        Task<IEnumerable<RegionModel>> GetAllAsync();
        Task<Region> GetRegionAsync(int regId);
        Task EditRegionAsync(RegionModel model);
        Task DeleteRegionAsync(int regId);
    }
}
