
namespace MigrationProgressApp
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    class Program
    {
        private static DateTime start = DateTime.Now;
        const int sleepTime = 13000;
        private long sourceCollectionCount = 0;
        private double currentPercentage = 0;
        private long prevDestinationCollectionCount = 0;
        private long currentDestinationCollectionCount = 0;
        private double totalInserted = 0;
   
        private static IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .AddEnvironmentVariables()
                .Build();

        static void Main(string[] args)
        {
            Program program = new Program();
            program.RunAsync().Wait();
        }

        public async Task RunAsync()
        {
            MigrationConfig configInstance = new MigrationConfig(config["EndPoint"], config["AuthKey"], config["SourceDatabase"], config["SourceCollection"],
                config["endPoint"], config["authKey"],
                config["TargetDatabase"], config["TargetCollection"]);

            while (true)
            {
                await TrackMigrationProgressAsync(configInstance);
                await Task.Delay(10000);
            }
        }

        private async Task TrackMigrationProgressAsync(MigrationConfig migrationConfig)
        {
            using (DocumentClient sourceClient = new DocumentClient(new Uri(migrationConfig.MonitoredUri),
                migrationConfig.MonitoredSecretKey))
            {
                sourceClient.ConnectionPolicy.RetryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 1000, MaxRetryWaitTimeInSeconds = 1000 };
                using (DocumentClient destinationClient = new DocumentClient(new Uri(migrationConfig.DestUri),
                    migrationConfig.DestSecretKey))
                {
                    destinationClient.ConnectionPolicy.RetryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 1000, MaxRetryWaitTimeInSeconds = 1000 };

                    RequestOptions options = new RequestOptions()
                    {
                        PopulateQuotaInfo = true,
                        PopulatePartitionKeyRangeStatistics = true
                    };

                    ResourceResponse<DocumentCollection> sourceCollection = await sourceClient.ReadDocumentCollectionAsync(
                                    UriFactory.CreateDocumentCollectionUri(migrationConfig.MonitoredDbName, migrationConfig.MonitoredCollectionName), options);

                    sourceCollectionCount = sourceCollection.Resource.PartitionKeyRangeStatistics
                        .Sum(pkr => pkr.DocumentCount);

                    ResourceResponse<DocumentCollection> destinationCollection = await destinationClient.ReadDocumentCollectionAsync(
                        UriFactory.CreateDocumentCollectionUri(migrationConfig.DestDbName, migrationConfig.DestCollectionName), options);

                    currentDestinationCollectionCount = destinationCollection.Resource.PartitionKeyRangeStatistics
                        .Sum(pkr => pkr.DocumentCount);

                    currentPercentage = sourceCollectionCount == 0 ? 100 : currentDestinationCollectionCount * 100.0 / sourceCollectionCount;

                    double currentRate = (currentDestinationCollectionCount - prevDestinationCollectionCount) * 1000.0 / sleepTime;
                    totalInserted += prevDestinationCollectionCount == 0 ? 0 : currentDestinationCollectionCount - prevDestinationCollectionCount;

                    DateTime currentTime = DateTime.UtcNow;
                    long totalSeconds = (long)((DateTime.Now - start).TotalMilliseconds) / 1000;
                    double averageRate = totalInserted * 1.0 / totalSeconds;
                    double eta = averageRate == 0 ? 0 : (sourceCollectionCount - currentDestinationCollectionCount) * 1.0 / (averageRate * 3600);

                    trackMetrics(sourceCollectionCount, currentDestinationCollectionCount, currentRate, averageRate, eta);

                    prevDestinationCollectionCount = currentDestinationCollectionCount;


                }
            }
        }

        private void trackMetrics(long sourceCollectionCount, long currentDestinationCollectionCount, double currentRate, double averageRate, double eta)
        {
            Console.WriteLine("CurrentPercentage = " + currentPercentage, currentPercentage);
            Console.WriteLine("ETA = " + eta);
            Console.WriteLine("Current rate = " + currentRate);
            Console.WriteLine("Average rate = " + averageRate);
            Console.WriteLine("Source count = " + sourceCollectionCount);
            Console.WriteLine("Destination count = " + currentDestinationCollectionCount);
            Console.WriteLine("***********************");
        }
    }

}