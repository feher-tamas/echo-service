using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoService.Configuration
{
    public static class ConfigRegistration
    {
        public static IServiceCollection AddConfigs(this IServiceCollection services,
            IConfiguration configurationSection)
        {

            services.AddOptions<WorkerConfig>()
                      .Configure(workerConfig =>
                      {
                          workerConfig.ServiceName = "echo";
                          workerConfig.WorkerIdPrefix = 'W';
                          workerConfig.WorkerNumber = 1;
                          workerConfig.BrokerAddress = "*";
                          workerConfig.BrokerPort = 5051;
                      })
                      .Bind(configurationSection.GetSection(nameof(WorkerConfig)));


            return services;
        }
    }
}
