using Duplex.Core.Models;
using Duplex.Infrastructure.Data.Models;

namespace Duplex.Core.Contracts
{
    public interface IPrizeService
    {
        Task AddPrizeAsync(AddPrizeModel model);
        Task<IEnumerable<PrizeModel>> GetAllAsync();
        Task<Prize> GetPrizeAsync(Guid pId);
        Task EditPrizeAsync(EditPrizeModel model);
        Task DeletePrizeAsync(Guid pId);
    }
}
