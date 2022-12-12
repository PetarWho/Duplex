using Duplex.Core.Common;
using Duplex.Core.Contracts;
using Duplex.Core.Models.Prize;
using Duplex.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Duplex.Core.Services
{
    public class PrizeService : IPrizeService
    {
        private readonly IRepository repo;

        public PrizeService(IRepository _repo)
        {
            repo = _repo;
        }

        public async Task AddPrizeAsync(AddPrizeModel model)
        {
            await repo.AddAsync<Prize>(new Prize()
            {
                Name = model.Name,
                Cost = model.Cost,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                CreatedOnUTC = DateTime.UtcNow,
            });

            await repo.SaveChangesAsync();
        }

        public async Task DeletePrizeAsync(Guid pId)
        {
            await repo.DeleteAsync<Prize>(pId);
            await repo.SaveChangesAsync();
        }

        public async Task EditPrizeAsync(EditPrizeModel model)
        {
            var prize = await repo.GetByIdAsync<Prize>(model.Id);
            
            prize.Name = model.Name;
            prize.Cost = model.Cost;
            prize.Description = model.Description;
            prize.ImageUrl = model.ImageUrl;
            prize.CreatedOnUTC = DateTime.UtcNow;

            await repo.SaveChangesAsync();
        }

        public async Task<bool> Exists(Guid pId)
        {
            return await repo.AllReadonly<Prize>().AnyAsync(e => e.Id == pId);
        }

        public async Task<IEnumerable<PrizeModel>> GetAllAsync()
        {
            return await repo.AllReadonly<Prize>().Select(p => new PrizeModel()
            {
                Id = p.Id,
                Name = p.Name,
                Cost = p.Cost,
                CreatedOnUTC = p.CreatedOnUTC,
                Description = p.Description,
                ImageUrl = p.ImageUrl
            }).ToListAsync();
        }

        public async Task<PrizeModel> GetPrizeAsync(Guid pId)
        {
            var prize = await repo.GetByIdAsync<Prize>(pId);

            return new PrizeModel()
            {
                Id = prize.Id,
                Name = prize.Name,
                Cost= prize.Cost,
                ImageUrl = prize.ImageUrl,
                Description = prize.Description,
                CreatedOnUTC = prize.CreatedOnUTC
            };
        }
    }
}
