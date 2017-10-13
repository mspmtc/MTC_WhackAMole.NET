using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhackAMole.KubeAdmin.Models;

namespace WhackAMole.KubeAdmin.Services
{
    internal interface IKubeResource
    {
        KubeMetaData MetaData { get; set; }
    }
}
