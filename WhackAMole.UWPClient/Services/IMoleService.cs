using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhackAMole.UWPClient.Models;

namespace WhackAMole.UWPClient.Services
{
    public interface IMoleService
    {
    
        Task<MoleState> GetStateUpdateAsync();
    }
}