using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhackAMole.UWPClient.Models;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace WhackAMole.UWPClient.Services
{
    // I don't like this but gets the job done
    public class AdminService : IAdminService
    {
        private static AdminService _instance;
        const string POD_API = "api/pods";
        const string NODE_API = "api/nodes";
        //private HttpClient _http;
        private static string _endpoint;


        private AdminService()
        {
            throw new Exception("Don't do this");
        }
        private AdminService(string endpoint)
        {
            _endpoint = endpoint;

        }

        private HttpClient CreateHttp()
        {
            var filter = new HttpBaseProtocolFilter();
            filter.MaxConnectionsPerServer = 20;
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
            return new HttpClient(filter);
        }

        public static AdminService Instance
        {
            get
            {
                if (_instance == null )
                    throw new Exception("Instance not created");
                return _instance;
            }
        }

        public static void Create(string endpoint)
        {
            if (_instance != null && endpoint != _endpoint)
                throw new Exception("Instance already created");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("endpoint cannot be null or empty");

            if (!Uri.IsWellFormedUriString(endpoint, UriKind.Absolute))
                throw new ArgumentException("Bad endpoint format");

            _instance = new AdminService(endpoint);
        }

        public async Task<List<KubePod>> GetPodsAsync(DateTimeOffset? since = null)
        {
            try
            {
                var _http = CreateHttp();
                if (since == null)
                    since = DateTimeOffset.MinValue;

                var uri = $"{_endpoint}/{POD_API}/molecloud";
                var json = await _http.GetStringAsync(new Uri(uri));
                var pods = JsonConvert.DeserializeObject<KubePod[]>(json);

                return pods.Where(p => p.StartTime >= since).ToList();
            }
            catch (Exception ex)

            {
                Debug.WriteLine("ERROR: GetPodsAsync Exception: " + ex.Message);
                return new List<KubePod>();
            }
        }

        public async Task<List<KubeNode>> GetNodesAsync()
        {
            try
            {
                var _http = CreateHttp();
                var uri = $"{_endpoint}/{NODE_API}";
                var json = await _http.GetStringAsync(new Uri(uri));
                var nodes = JsonConvert.DeserializeObject<KubeNode[]>(json);
                return nodes.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: GetNodesAsync: {ex.Message} ");
                return new List<KubeNode>();
            }
        }

      

        public async Task<bool> DeletePodAsync(string molename)
        {
            var _http = CreateHttp();
            Debug.WriteLine($">>>deleting mole {molename}: ");
            try
            {
                var result = await _http.DeleteAsync(new Uri($"{_endpoint}/{POD_API}/{molename}"));
                Debug.WriteLine(result.IsSuccessStatusCode);


                return result.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
