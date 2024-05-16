using Azure.Storage.Blobs;
using Azure;

namespace MouseTagProject.Context.Seeders
{
    public class ContainerCreator
    {

        //-------------------------------------------------
        // Create a container
        //-------------------------------------------------
        public static async Task<BlobContainerClient> CreateSampleContainerAsync(BlobServiceClient blobServiceClient, string input)
        {
            // Name the sample container based on new GUID to ensure uniqueness.
            // The container name must be lowercase.
            //string containerName = "container-" + Guid.NewGuid();
            string containerName = input;
            try
            {
                // Create the container
                BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(containerName);

                if (await container.ExistsAsync())
                {
                    Console.WriteLine("Created container {0}", container.Name);
                    return container;
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}",
                                    e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
            }
            return null;
        }
    }
}
