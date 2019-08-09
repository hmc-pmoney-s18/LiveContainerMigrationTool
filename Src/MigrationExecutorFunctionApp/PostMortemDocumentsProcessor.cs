/// <summary>
/// Listens to the post mortem queue and tries to
/// re-insert the elements in the queue back in the
/// cosmos db target collection
/// </summary>
namespace MigrationExecutorFunctionApp
{
    using System;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Microsoft.Azure.Documents;
    using System.Threading.Tasks;
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
            [QueueTrigger("%QueueName%", Connection = "QueueConnectionString")]Document myQueueItem, ILogger log)
        {

            try
            {
                await client.UpsertDocumentAsync(targetContainerLink, myQueueItem);

            }
            catch (DocumentClientException e)
            {
                log.LogError(e, e.Message);
            }

        }
    }
}
