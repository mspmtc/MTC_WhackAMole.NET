using System.IO;
using System.Net.Http;
using WhackAMole.MoleCloud.Services;

namespace WhackAMole.MoleCloud.Services
{
    public abstract class BaseAuthenticationProvider 
    {
        protected HttpClient CreateBaseConnection()
        {
            var handler = new HttpClientHandler();


            var http = new HttpClient(handler);
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            http.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
            return http;
        }
    }

    public class LocalServiceTokenProvider : BaseAuthenticationProvider, IAuthenticationProvider
    {
        private readonly string _token;

        public LocalServiceTokenProvider()
        {
            _token = GetToken();
        }

        public HttpClient GetConnection() {

            var http = CreateBaseConnection();
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}"); 
            return http;

            
          
        }

        

        private string GetToken()
        {
            if (!File.Exists("/var/run/secrets/kubernetes.io/serviceaccount/token"))
                throw new FileNotFoundException();

            var token = File.ReadAllText("/var/run/secrets/kubernetes.io/serviceaccount/token");
            return token;
        }
    
    }
}