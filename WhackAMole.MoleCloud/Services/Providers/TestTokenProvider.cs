using System.Net.Http;

namespace WhackAMole.MoleCloud.Services
{
    internal class TestTokenProvider : IAuthenticationProvider
    {
        private readonly string _token;

        public TestTokenProvider()
        {
            _token = "<token from kubernetes config>";
        }

        public HttpClient GetConnection() {
            var handler = new HttpClientHandler();


            var http = new HttpClient(handler);
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            http.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}"); 
            return http;
        }
    }
}