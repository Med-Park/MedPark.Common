using Consul;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MedPark.Common.Consul
{
    public class ConsulServices : IConsulServices
    {
        private readonly IConsulClient _client;
        private readonly IDictionary<string, ISet<string>> _servicesInUse = new Dictionary<string, ISet<string>>();

        private readonly Random _random = new Random();

        public ConsulServices(IConsulClient client)
        {
            _client = client;
        }

        public async Task<AgentService> GetAsync(string name)
        {
            var allRegisteredServices = await _client.Agent.Services();

            var registeredServices = allRegisteredServices.Response?.Where(s => s.Value.Service.Equals(name, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value).ToList();

            if (!registeredServices.Any())
                return null;
            
            if (!_servicesInUse.ContainsKey(name))
            {
                _servicesInUse[name] = new HashSet<string>();
            }

            if (allRegisteredServices.Response.Count == _servicesInUse[name].Count)
            {
                ClearServiceFromRegistry(name);
            }

            return GetRandomInstance(registeredServices, name);
        }

        private AgentService GetRandomInstance(IList<AgentService> services, string serviceName)
        {
            AgentService servToUse = null;

            var servicesNotInUse = services.Where(s => !_servicesInUse[serviceName].Contains(s.ID)).ToList();

            if(servicesNotInUse.Any())
            {
                servToUse = servicesNotInUse[_random.Next(0, servicesNotInUse.Count)];
            }
            else
            {
                servToUse = services.First();
                ClearServiceFromRegistry(serviceName);
            }

            _servicesInUse[serviceName].Add(servToUse.ID);

            return servToUse;
        }

        private void ClearServiceFromRegistry(string serviceName)
        {
            _servicesInUse[serviceName].Clear();
        }
    }
}
