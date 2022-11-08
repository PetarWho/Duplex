using Duplex.Core.Models;

namespace Duplex.Core.Contracts
{
    public interface IRegionService
    {
        Task AddRegionAsync(RegionModel model);
    }
}
