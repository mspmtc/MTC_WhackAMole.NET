using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WhackAMole.MoleCloud.Services
{
    public interface IAuthenticationProvider
    {
        HttpClient GetConnection();
    }
}
