using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhackAMole.KubeAdmin.Services.Models;

namespace WhackAMole.KubeAdmin.Services
{
    internal class NodesRequest : KubeBaseRequest<KubeNode>, INodesRequest
    {
        public NodesRequest(IAuthenticationProvider auth) : base(auth)
        {

        }

        public async Task<KubeNode[]> GetAllAsync()
        {
            var list = await GetAsync<KubeNodeList>("nodes");

            return list.Items;
        }

        public async Task<bool> DeleteAsync(string name)
        {
            return await base.DeleteAsync($"nodes/{name}");
        }
    }
}
