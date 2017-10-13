using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WhackAMole.MoleCloud.Services
{
    public class KubeRequestBuilder
    {
        private IAuthenticationProvider _auth;

        public KubeRequestBuilder(IAuthenticationProvider auth)
        {
            _auth = auth;
        }
        internal T Create<T>() where T : IKubeRequest
        {
            var table = (IKubeRequest)Activator.CreateInstance(
                            typeof(T),
                            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null,
                            new object[] { _auth }, null);

            return (T) table;
        }
    }
}
