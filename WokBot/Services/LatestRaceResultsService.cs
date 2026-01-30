using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WokBot.Interfaces;
using WokBot.Models.Database;
using WokBot.Models.OpenF1Api;

namespace WokBot.Services
{
    public class LatestRaceResultsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDatabaseContextFactory _databaseContextFactory;

        public LatestRaceResultsService(IHttpClientFactory httpClientFactory, IDatabaseContextFactory databaseContextFactory)
        {
            _httpClientFactory = httpClientFactory;
            _databaseContextFactory = databaseContextFactory;
        }

        public async Task FetchLatestRaceResultsAsync()
        {
            using var httpClient = _httpClientFactory.CreateClient();

            using var databaseContext = _databaseContextFactory.Create();

            var latestSessionInDatabase = await databaseContext.Set<LatestRaceData>().OrderByDescending(x => x.Id).FirstOrDefaultAsync();

            var latestSession = await GetLatestSessionAsync(httpClient);
            if(latestSessionInDatabase.RaceId >= latestSession.session_key)
            {
                return;
            }


        }

        private async Task<Session> GetLatestSessionAsync(HttpClient httpClient)
        {
            var currentYear = DateTime.UtcNow.Year;

            var sessionsFromApi = await httpClient.GetStringAsync($"https://api.openf1.org/v1/sessions?session_type=Race&year={currentYear}");

            var sessions = JsonConvert.DeserializeObject<List<Session>>(sessionsFromApi);

            var latestSession = sessions.MaxBy(x => x.session_key);

            return latestSession;
        }
    }
}
