using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoService.Configuration
{
    public class WorkerConfig
    {
        public string ServiceName { get; set; }
        public char WorkerIdPrefix { get; set; }
        public int WorkerNumber { get; set; }
        public string BrokerAddress { get; set; }
        public int BrokerPort { get; set; }
    }
}
