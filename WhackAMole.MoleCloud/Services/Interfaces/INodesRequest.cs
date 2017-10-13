using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhackAMole.MoleCloud.Services.Models;

namespace WhackAMole.MoleCloud.Services
{
    interface INodesRequest
    {
        Task<KubeNode[]> GetAllAsync();
    }
}
