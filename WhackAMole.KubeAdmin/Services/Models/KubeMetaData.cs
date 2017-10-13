using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WhackAMole.KubeAdmin.Models
{
    public class KubeMetaData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("uid")]
        public string Uid { get; set; }
        [JsonProperty("selfLink")]
        public string SelfLink { get; set; }
        [JsonProperty("resourceVersion")]
        public string ResourceVersion { get; set; }
    }
}
