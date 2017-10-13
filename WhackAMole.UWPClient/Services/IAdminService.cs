using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhackAMole.UWPClient.Models;

namespace WhackAMole.UWPClient.Services
{
    public interface IAdminService
    {
        Task<bool> DeletePodAsync(string molename);
        Task<List<KubeNode>> GetNodesAsync();
        Task<List<KubePod>> GetPodsAsync(DateTimeOffset? since = null);
    }
}