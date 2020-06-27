using System;
using System.Collections.Generic;
using System.Text;

namespace MedPark.Common.Consul
{
    public class ConsulOptions
    {
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool PingEnabled { get; set; }
    }
}
