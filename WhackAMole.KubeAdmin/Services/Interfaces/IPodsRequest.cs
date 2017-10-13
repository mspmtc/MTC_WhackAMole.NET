using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhackAMole.KubeAdmin.Models;
using WhackAMole.KubeAdmin.Services.Models;

namespace WhackAMole.KubeAdmin.Services
{
    internal interface IPodsRequest : IKubeRequest
    {
        Task<KubePod[]> GetAllAsync(string appname = "");
    }

   
}
