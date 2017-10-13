using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhackAMole.MoleCloud.Models;

namespace WhackAMole.MoleCloud.Services
{
    internal interface IKubeResource
    {
        KubeMetaData MetaData { get; set; }
    }
}
