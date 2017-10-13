using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhackAMole.MoleCloud.Models;

namespace WhackAMole.MoleCloud.Services.Models
{
   

    public class KubeNode : IKubeResource
    {
        public KubeMetaData MetaData { get; set; }
        public KubeSpec Spec { get; set; }
    
    }
    public class KubeNodeList : IKubeResource
    {
        public KubeMetaData MetaData { get; set; }
        [JsonProperty("items")]
        public KubeNode[] Items;
    }

}
