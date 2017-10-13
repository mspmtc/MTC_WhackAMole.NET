using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhackAMole.MoleCloud.Models
{
    public class MoleState
    {

        public string Name { get; set; }
        public string CurrentChar { get; set; }
        public string Color { get; set; }

    }

    public class Pod
    {
        public string Name { get; set; }

        public string Uid { get; set; }

        public string Host { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public string Phase { get; set; }
    }
}
