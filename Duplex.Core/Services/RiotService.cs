using Duplex.Core.Common.Constants;
using Duplex.Core.Contracts;
using Duplex.Core.Models.RiotDtos;
using Duplex.Core.Models.RiotDTOs;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Duplex.Core.Services
{
    public class RiotService : IRiotService
    {
        /// <summary>
        /// Get the last X games
        /// </summary>
        /// <param name="puuid">The players UUID</param>
        /// <param name="count">Number of games to take (up to 100)</param>
        /// <param name="server">Search in the api provided for the given server. Options: "americas", "europe", "asia", "sea"</param>
        /// <returns>string[]</returns>
        public async Task<string[]> GetLastGamesIdsAsync(string puuid, int count, string server)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException("Invalid count");
            }
            var url = $@"https://{server.ToLower()}.api.riotgames.com/lol/match/v5/matches/by-puuid/{puuid}/ids?start=0&count={count}&api_key={RiotAPIConst.APIKey}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var responseDto = JsonConvert.DeserializeObject<string[]>(await client.GetStringAsync(url));

            return responseDto ?? Array.Empty<string>();
        }

        /// <summary>
        /// Get the whole match as object
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="server">Search in the api provided for the given server. Options: "americas", "europe", "asia", "sea"</param>
        /// <param name="region">Options: "EUN1", "EUW1", "BR1", "JP1", "KR", "LA1", "LA2", "NA1", "OC1", "RU", "TR1"</param>
        /// <returns>Root object</returns>
        public async Task<Root?> GetMatchByMatchIdAsync(string matchId, string server, string region)
        {
            var url = $@"https://{server.ToLower()}.api.riotgames.com/lol/match/v5/matches/{region.ToUpper()}_{matchId}?api_key={RiotAPIConst.APIKey}";

            var client = new HttpClient();
            client.BaseAddress =new Uri(url);
            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var responseDto = JsonConvert.DeserializeObject<Root>(await client.GetStringAsync(url));

            return responseDto;
        }

        /// <summary>
        /// Get Player's UUID By his Summoner name
        /// </summary>
        /// <param name="summonerName">Player's Summoner name</param>
        /// <param name="region">Options: "EUN1", "EUW1", "BR1", "JP1", "KR", "LA1", "LA2", "NA1", "OC1", "RU", "TR1"</param>
        /// <returns>string</returns>
        public async Task<string> GetUserPUUIDBySummonerNameAsync(string summonerName, string region)
        {
            var url = @$"https://{region.ToLower()}.api.riotgames.com/lol/summoner/v4/summoners/by-name/{summonerName}?api_key={RiotAPIConst.APIKey}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var responseDto = JsonConvert.DeserializeObject<PUUIDdto>(await client.GetStringAsync(url));

            return responseDto?.puuid ?? string.Empty;
        }

        /// <summary>
        /// Check if player has equipped the verifaction icon
        /// </summary>
        /// <param name="summonerName">Player's Summoner name</param>
        /// <param name="region">Options: "EUN1", "EUW1", "BR1", "JP1", "KR", "LA1", "LA2", "NA1", "OC1", "RU", "TR1"</param>
        /// <returns>bool</returns>
        public async Task<bool> VerifyPUUIDBySummonerIconAsync(string summonerName, string region)
        {
            var url = @$"https://{region.ToLower()}.api.riotgames.com/lol/summoner/v4/summoners/by-name/{summonerName}?api_key={RiotAPIConst.APIKey}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var responseDto = JsonConvert.DeserializeObject<PUUIDdto>(await client.GetStringAsync(url));

            return responseDto?.ProfileIconId == 12;
        }
    }
}
