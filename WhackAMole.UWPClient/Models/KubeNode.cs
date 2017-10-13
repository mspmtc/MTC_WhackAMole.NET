using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhackAMole.UWPClient.Models
{
    public class KubeNode
    {
        public string Name { get; set; }

        public string Uid { get; set; }

        public DateTimeOffset StartTime { get; set; }
    }
}
