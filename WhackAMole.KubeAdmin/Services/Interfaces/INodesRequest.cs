using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhackAMole.KubeAdmin.Services.Models;

namespace WhackAMole.KubeAdmin.Services
{
    interface INodesRequest
    {
        Task<KubeNode[]> GetAllAsync();
    }
}
