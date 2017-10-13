using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhackAMole.KubeAdmin.Models;

namespace WhackAMole.KubeAdmin.Services.Models
{
    public class KubePodStatus
    {
        [JsonProperty("phase")]
        public string Phase { get; set; }

        [JsonProperty("startTime")]
        public string StartTime { get; set; }
    }

    public class KubePod : IKubeResource
    {
        public KubeMetaData MetaData { get; set; }
        public KubeSpec Spec { get; set; }
        public KubePodStatus Status { get; set; }
    }

    public class KubePodList : IKubeResource
    {
        public KubeMetaData MetaData { get; set; }
        [JsonProperty("items")]
        public KubePod[] Items;
    }
}
