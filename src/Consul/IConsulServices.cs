using Consul;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedPark.Common.Consul
{
    public interface IConsulServices
    {
        Task<AgentService> GetAsync(string name);
    }
}
