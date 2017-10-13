using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhackAMole.UWPClient.Models
{
    public class KubePodComparer : EqualityComparer<KubePod>
    {
      

        public override bool Equals(KubePod x, KubePod y)
        {
            bool result = false;
            if (x == null && y == null)
                result =  true;
            else 
                if (x == null || y == null)
                    result = false;
                else
                    result = (x.Name == y.Name);

            return result;
        }

       

        public override int GetHashCode(KubePod obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public class KubePod
    {
        public string Name { get; set; }

        public string Uid { get; set; }

        public string Host { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public string Phase { get; set; }

    }

   
}
