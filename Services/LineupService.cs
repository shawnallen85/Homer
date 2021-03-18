using System.Collections.Generic;
using System.Threading.Tasks;
using Homer.Models;
using RestSharp;

namespace Homer.Services
{
    public static class LineupService
    {
        public static async Task<IEnumerable<Lineup>> GetItemsAsync(RestClient client)
        {
            var request = new RestRequest("lineup.json", DataFormat.Json);
            return await client.GetAsync<IEnumerable<Lineup>>(request);
        }
        
        public static IEnumerable<Lineup> GetItems(RestClient client)
        {
            var request = new RestRequest("lineup.json", DataFormat.Json);
            var response = client.Get<Lineup[]>(request);
            return !response.IsSuccessful ? null : response.Data;
        }
    }
}