using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WhackAMole.KubeAdmin.Services
{
    public interface IAuthenticationProvider
    {
        HttpClient GetConnection();
    }
}
