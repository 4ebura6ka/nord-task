using PartyCli.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartyCli.API
{
    public interface IServerService
    {
        Task<List<ServerModel>> GetAllServersByCountry(int countryId);
        Task<List<ServerModel>> GetAllServersByProtocol(int vpnProtocol);
        Task<List<ServerModel>> GetAllServers();
    }
}