using Duplex.Core.Models;

namespace Duplex.Core.Contracts
{
    public interface IRegionService
    {
        Task AddRegionAsync(RegionModel model);
        Task<IEnumerable<RegionModel>> GetAllAsync();
        Task<RegionModel> GetRegionAsync(int regId);
        Task EditRegionAsync(RegionModel model);
        Task DeleteRegionAsync(int regId);
        Task<bool> Exists(int regId);
    }
}
