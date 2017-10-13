using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WhackAMole.KubeAdmin.Services.Models
{
    
    public class KubeSpec
    {
     
        [JsonProperty("nodeName")]
        public string NodeName { get; set; }
    
    }
}
