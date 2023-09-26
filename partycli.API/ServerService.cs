using System;
using System.Net.Http;

namespace partycli.API
{
	public class ServerService
	{

        private static readonly HttpClient client = new HttpClient();

        public ServerService()
		{
		}

        public string GetAllServersListAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers");
            var response = client.SendAsync(request).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return responseString;
        }

        public string GetAllServerByCountryListAsync(int countryId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers?filters[servers_technologies][id]=35&filters[country_id]=" + countryId);
            var response = client.SendAsync(request).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return responseString;
        }

        public string GetAllServerByProtocolListAsync(int vpnProtocol)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers?filters[servers_technologies][id]=" + vpnProtocol);
            var response = client.SendAsync(request).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return responseString;
        }

    }
}

