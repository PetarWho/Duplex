using Duplex.Core.Models.RiotDTOs;

namespace Duplex.Core.Contracts
{
    public interface IRiotService
    {
        Task<string> GetUserPUUIDBySummonerNameAsync(string summonerName, string region);
        Task<bool> VerifyPUUIDBySummonerIconAsync(string summonerName, string region);
        Task<Root?> GetMatchByMatchIdAsync(string matchId, string server, string region);
        Task<string[]> GetLastGamesIdsAsync(string puuid, int count, string server);
    }
}
