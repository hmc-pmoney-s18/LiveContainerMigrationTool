using System;
using System.Linq;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.CosmosDB.BulkExecutor;
[assembly: FunctionsStartup(typeof(MigrationExecutorFunctionApp.Startup))]

namespace MigrationExecutorFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string endPoint = Environment.GetEnvironmentVariable($"{"endPoint"}");
            string authKey = Environment.GetEnvironmentVariable($"{"authKey"}");


            string database = Environment.GetEnvironmentVariable($"{"targetDatabase"}");
            string collection = Environment.GetEnvironmentVariable($"{"targetCollection"}");

            Uri targetContainerUri = UriFactory.CreateDocumentCollectionUri(database, collection);

            DocumentClient client = GetCustomClient(endPoint, authKey);
            DocumentCollection customTargetContainer = GetTargetCollection(database, collection, client);
            IBulkExecutor bulkExecutor = GetBulkExecutor(client, customTargetContainer);

            client.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 30;
            client.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 30;

            bulkExecutor.InitializeAsync().GetAwaiter().GetResult();

            client.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 0;
            client.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 0;

            builder.Services.AddSingleton<DocumentClient>(client);
            builder.Services.AddSingleton<Uri>(targetContainerUri);
            builder.Services.AddSingleton<IBulkExecutor>(bulkExecutor);
        }

        private static DocumentClient GetCustomClient(string endPoint, string authKey)
        {
            DocumentClient customClient = new DocumentClient(
                new Uri(endPoint),
                authKey,
                new ConnectionPolicy()
                {
                    ConnectionMode = Microsoft.Azure.Documents.Client.ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp,
                    RetryOptions = new RetryOptions()
                    {
                        MaxRetryAttemptsOnThrottledRequests = 10,
                        MaxRetryWaitTimeInSeconds = 30
                    }
                });

            return customClient;
        }

        private static DocumentCollection GetTargetCollection(string database, string collection, DocumentClient client)
        {
            return client.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(database))
                .Where(c => c.Id == collection).AsEnumerable().FirstOrDefault();
        }

        private static IBulkExecutor GetBulkExecutor(DocumentClient client, DocumentCollection targetCollection)
        {
            client.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 30;
            client.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 30;

            IBulkExecutor bulkExecutor = new BulkExecutor(client, targetCollection);

            client.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 0;
            client.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 0;

            return bulkExecutor;
        }


    }
}
