using Newtonsoft.Json;
using PartyCli.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PartyCli.API
{
    public class ServerService : IServerService, IDisposable
    {
        private readonly HttpClient _httpClient;

        public ServerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ServerModel>> GetAllServers()
        {
            var response = await _httpClient.GetStringAsync("servers");
            var servers = Deserialize(response);
            return servers;
        }

        public async Task<List<ServerModel>> GetAllServersByCountry(int countryId)
        {
            var response = await _httpClient.GetStringAsync($"servers?filters[servers_technologies][id]=35&filters[country_id]={countryId}");
            var servers = Deserialize(response);
            return servers;
        }

        public async Task<List<ServerModel>> GetAllServersByProtocol(int vpnProtocol)
        {
            var response = await _httpClient.GetStringAsync($"servers??filters[servers_technologies][id]={vpnProtocol}");
            var servers = Deserialize(response);
            return servers;
        }

        public void Dispose() => _httpClient?.Dispose();

        private List<ServerModel> Deserialize(string response)
        {
            try
            {
                var servers = JsonConvert.DeserializeObject<List<ServerModel>>(response);
                return servers;
            }
            catch { }

            return null;
        }

    }
}

