namespace MigrationExecutorFunctionApp
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    
    public class PostMortemDocumentsProcessor
    {
        private Uri targetContainerLink;

        public PostMortemDocumentsProcessor(Uri targetContainerLink)
        {
            this.targetContainerLink = targetContainerLink;
        }
         
        [FunctionName("Function2")]
        public async Task Run(
            [CosmosDB("%TargetDatabase%", "%TargetCollection%", ConnectionStringSetting = "CosmosDB")]IDocumentClient client,
            [QueueTrigger("%QueueName%", Connection = "QueueConnectionString")]Document myQueueItem, 
            ILogger log)
        {
            try
            {
                await client.UpsertDocumentAsync(this.targetContainerLink, myQueueItem);
            }
            catch (DocumentClientException e)
            {
                log.LogError(e, e.Message);
            }
        }
    }
}
