using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationProgressApp
{
    public class MigrationConfig
    {
        [JsonProperty("monitoredUri")]
        public string MonitoredUri { get; set; }

        [JsonProperty("monitoredSecretKey")]
        public string MonitoredSecretKey { get; set; }

        [JsonProperty("monitoredDbName")]
        public string MonitoredDbName { get; set; }

        [JsonProperty("monitoredCollectionName")]
        public string MonitoredCollectionName { get; set; }
        [JsonProperty("destUri")]
        public string DestUri { get; set; }

        [JsonProperty("destSecretKey")]
        public string DestSecretKey { get; set; }

        [JsonProperty("destDbName")]
        public string DestDbName { get; set; }

        [JsonProperty("destCollectionName")]
        public string DestCollectionName { get; set; }

        public MigrationConfig(string MonitoredUri, string MonitoredSecretKey, string MonitoredDbName, string MonitoredCollectionName,
            string DestUri, string DestKey, string DestDbName, string DestCollectionName)
        {
            this.MonitoredUri = MonitoredUri;
            this.MonitoredSecretKey = MonitoredSecretKey;
            this.MonitoredDbName = MonitoredDbName;
            this.MonitoredCollectionName = MonitoredCollectionName;
            this.DestUri = DestUri;
            this.DestSecretKey = DestKey;
            this.DestDbName = DestDbName;
            this.DestCollectionName = DestCollectionName;
        }
      
    }
    
}
