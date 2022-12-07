using Duplex.Core.Models.Prize;

namespace Duplex.Core.Contracts
{
    public interface IPrizeService
    {
        Task AddPrizeAsync(AddPrizeModel model);
        Task<IEnumerable<PrizeModel>> GetAllAsync();
        Task<PrizeModel> GetPrizeAsync(Guid pId);
        Task EditPrizeAsync(EditPrizeModel model);
        Task DeletePrizeAsync(Guid pId);
        Task<bool> Exists(Guid pId);
    }
}
