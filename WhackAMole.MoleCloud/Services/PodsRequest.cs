using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhackAMole.MoleCloud.Services.Models;

namespace WhackAMole.MoleCloud.Services
{
    internal class PodsRequest : KubeBaseRequest<KubePod>, IPodsRequest 
    {
        public PodsRequest(IAuthenticationProvider auth) : base (auth)
        {

        }

        public async Task<KubePod[]> GetAllAsync(string appname = "")
        {
            KeyValuePair<string, string>[] query = null;
            if (!string.IsNullOrWhiteSpace(appname))
                query = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("labelSelector",$"app={appname}")};

            var list = await GetAsync<KubePodList>("pods", queryvalues: query);
            
            return list.Items;
        }

        public async Task<bool> DeleteAsync(string name)
        {
            return await base.DeleteAsync($"pods/{name}","default");
        }
    }
}
