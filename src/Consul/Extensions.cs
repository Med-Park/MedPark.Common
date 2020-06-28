using Consul;
using MedPark.Common.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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

            serviceCollection.Configure<ConsulOptions>(configuration.GetSection("Consul"));
            serviceCollection.AddTransient<IConsulServices, ConsulServices>();
            serviceCollection.AddTransient<ConsulServiceDiscoveryMessageHandler>();
            serviceCollection.AddHttpClient<IConsulHttpClient, ConsulHttpClient>()
                .AddHttpMessageHandler<ConsulServiceDiscoveryMessageHandler>();

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

                Guid serviceId = Guid.NewGuid();
                string consulServiceID = $"{config.Service}:{serviceId}";


                var client = scope.ServiceProvider.GetService<IConsulClient>();

                var consulServiceRistration = new AgentServiceRegistration
                {
                    Name = config.Service,
                    ID = consulServiceID,
                    Address = config.Address,
                    Port = config.Port,
                    //TODO : Add Tags Tags = fabioOptions.Value.Enabled ? GetFabioTags(serviceName, fabioOptions.Value.Service) : null
                };

                if (config.PingEnabled)
                {
                    var healthService = scope.ServiceProvider.GetService<HealthCheckService>();

                    if (healthService != null)
                    {
                        var scheme = config.Address.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)
                       ? string.Empty
                       : "http://";
                        var check = new AgentServiceCheck
                        {
                            Interval = TimeSpan.FromSeconds(5),
                            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                            HTTP = $"{scheme}{config.Address}{(config.Port > 0 ? $":{config.Port}" : string.Empty)}/health"
                        };

                        consulServiceRistration.Checks = new[] { check };
                    }
                    else
                    {
                        throw new MedParkException("consul_check_initialization_exception", "Please ensure that Healthchecks has been added before adding checks to Consul.");
                    }
                }

                client.Agent.ServiceRegister(consulServiceRistration);

                return consulServiceID;
            }
        }


    }
}
