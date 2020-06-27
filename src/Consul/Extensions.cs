using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedPark.Common.Consul
{
    public static class Extensions
    {
        public static IServiceCollection AddConsul(this IServiceCollection serviceCollection)
        {
            IConfiguration configuration;
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }
            ConsulOptions consulConfigOptions = configuration.GetOptions<ConsulOptions>("Consul");

            return serviceCollection.AddSingleton<IConsulClient>(c => new ConsulClient(cfg =>
            {
                if (!string.IsNullOrEmpty(consulConfigOptions.Host))
                {
                    cfg.Address = new Uri(consulConfigOptions.Host);
                }
            }));
        }

        public static string UseConsul(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var Iconfig = scope.ServiceProvider.GetService<IConfiguration>();

                var config = Iconfig.GetOptions<ConsulOptions>("Consul");
                var appOptions = Iconfig.GetOptions<AppOptions>("App");

                if (!config.Enabled)
                    return String.Empty;

                string serviceName = $"{appOptions.Name}";
                Guid serviceId = Guid.NewGuid();
                string consulServiceID = $"{serviceName}:{serviceId}";


                var client = scope.ServiceProvider.GetService<IConsulClient>();

                var consulServiceRistration = new AgentServiceRegistration
                {
                    Name = serviceName,
                    ID = consulServiceID,
                    Address = config.Host,
                    Port = config.Port,
                    //TODO : Add Tags Tags = fabioOptions.Value.Enabled ? GetFabioTags(serviceName, fabioOptions.Value.Service) : null
                };

                if (config.PingEnabled)
                {
                    var check = new AgentServiceCheck
                    {
                        Interval = TimeSpan.FromSeconds(5),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                        HTTP = config.Host
                    };

                    consulServiceRistration.Checks = new[] { check };
                }

                client.Agent.ServiceRegister(consulServiceRistration);

                return consulServiceID;
            }
        }


    }
}
