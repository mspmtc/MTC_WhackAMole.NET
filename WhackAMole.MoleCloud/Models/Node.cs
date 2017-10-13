using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhackAMole.MoleCloud.Models
{
    public class Node
    {
        public string Name { get; set; }

        public string Uid { get; set; }

        public DateTimeOffset StartTime { get; set; }
    }
}
