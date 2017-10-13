using System.IO;
using System.Net.Http;
using WhackAMole.KubeAdmin.Services;

namespace WhackAMole.KubeAdmin.Services
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
                return "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJrdWJlcm5ldGVzL3NlcnZpY2VhY2NvdW50Iiwia3ViZXJuZXRlcy5pby9zZXJ2aWNlYWNjb3VudC9uYW1lc3BhY2UiOiJkZWZhdWx0Iiwia3ViZXJuZXRlcy5pby9zZXJ2aWNlYWNjb3VudC9zZWNyZXQubmFtZSI6ImRlZmF1bHQtdG9rZW4tNG5kN3MiLCJrdWJlcm5ldGVzLmlvL3NlcnZpY2VhY2NvdW50L3NlcnZpY2UtYWNjb3VudC5uYW1lIjoiZGVmYXVsdCIsImt1YmVybmV0ZXMuaW8vc2VydmljZWFjY291bnQvc2VydmljZS1hY2NvdW50LnVpZCI6ImMwYWJkOTFjLThmMWUtMTFlNy05Y2Y5LTAwMGQzYTYyMTE0OSIsInN1YiI6InN5c3RlbTpzZXJ2aWNlYWNjb3VudDpkZWZhdWx0OmRlZmF1bHQifQ.NMs9k_Usv1TvhHLW3snzkDNHLufaPFThRq_pdeckVzBOilJQ7yrhYFrWSYBigq60XnO7TwtH4ks9dB1vLsZhoAP62WmuXq8g-IiBQnAE2SBI_yMn85AkdDCKxoDJ8Q6qTh2RXBnutOftUcCoahsxZodQGggG1mO7mnEEbM4LsSIo2OSHuD-ijNa0blmFjGUBS7tDsRR0NAQVWgBaNUUbgeTKSiT6ddIIVkup0nfNvumP_Tpoq8MUPlYrse8vdC2Mzk7As93cERmDs5aE_mwBk58e-_uPL_arww6ZboTigCJMXVpEdOjMrR9Pzg0CTjaX8UK30sPzadQqX_bHTgmxLw";

            var token = File.ReadAllText("/var/run/secrets/kubernetes.io/serviceaccount/token");
            return token;
        }
    
    }
}