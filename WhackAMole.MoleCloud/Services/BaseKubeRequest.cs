using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;

namespace WhackAMole.MoleCloud.Services
{
    internal class KubeBaseRequest<T> : IKubeRequest where T : IKubeResource
    {
        
#if DEBUG
        const string BASE_URL = "https://molecluste-whackmole-150072mgmt.northcentralus.cloudapp.azure.com";
#endif
#if RELEASE
        const string BASE_URL = "https://kubernetes";
#endif

        const string API_VERSION = "api/v1";
      
        private readonly IAuthenticationProvider _auth;

        
        public KubeBaseRequest(IAuthenticationProvider authProvider)
        {
            _auth = authProvider;
        }

        protected async Task<string> GetAsync(string api, string nameSpace = "", KeyValuePair<string, string>[] queryvalues = null)
        {
            var http = _auth.GetConnection();
            var request = CreateRequest(api, nameSpace, queryvalues);
            var result = await http.GetStringAsync(request);
            return result;
        }

        protected async Task<T> GetAsync<T>(string api, string nameSpace = "", KeyValuePair<string,  string>[] queryvalues = null) where T : class
        {
            var json = await GetAsync(api, queryvalues: queryvalues);
            return JsonConvert.DeserializeObject<T>(json);
        }

        protected async Task<bool> DeleteAsync(string api, string nameSpace = "")
        {
            var http = _auth.GetConnection();
            var request = CreateRequest(api, nameSpace);
            var result = await http.DeleteAsync(request);

            return result.IsSuccessStatusCode;
        }

        private static string CreateRequest(string api, string nameSpace = "", KeyValuePair<string, string>[] queryvalues = null)
        {
            var request = (nameSpace == "") ? $"{BASE_URL}/{API_VERSION}/{api}" : $"{BASE_URL}/{API_VERSION}/namespaces/{nameSpace}/{api}";
            if (queryvalues == null)
                return request;
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var queryvalue in queryvalues)
                query[queryvalue.Key] = queryvalue.Value;

            return $"{request}?{query.ToString()}";
        }
    }
}
