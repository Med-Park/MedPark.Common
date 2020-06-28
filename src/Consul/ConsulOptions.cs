using System;
using System.Collections.Generic;
using System.Text;

namespace MedPark.Common.Consul
{
    public class ConsulOptions
    {
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public string Service { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public bool PingEnabled { get; set; }
        public int RequestRetries { get; set; }
        public bool SkipLocalhostDockerDnsReplace { get; set; }

        public List<ConsulService> Services { get; set; }
    }

    public class ConsulService
    {
        public string Name { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
