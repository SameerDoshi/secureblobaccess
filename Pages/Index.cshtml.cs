using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.Globalization;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
namespace StorageAuthDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private string storageConnectionString = "Placeholder";
        private string containerName = "data";
        public IList<BlobItem> blobs { get; set; }
        public UriBuilder sasUri { get; set; }
        public string sasToken { get; set; }
        public string blobURI { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            blobs = new List<BlobItem>();
            BlobContainerClient bcc = new BlobContainerClient(storageConnectionString, containerName);
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            CloudBlobContainer container= serviceClient.GetContainerReference(containerName);
            
            blobURI = bcc.Uri.AbsoluteUri.ToString();
            
             foreach (BlobItem blob in bcc.GetBlobs())
            {
                blobs.Add(blob);              
            }
            
           
            
            string StorageAccountKey = "Placeholder";
            string StorageAccountName = "Placeholder";
            
            sasUri = new UriBuilder(blobURI);
            //sasUri.Query=sas.ToSasQueryParameters(credential).ToString();
            sasToken = GetContainerSasUri(container);
            

        }
        private static string GetContainerSasUri(CloudBlobContainer container, string storedPolicyName = null)
        {
            string sasContainerToken;

            // If no stored policy is specified, create a new access policy and define its constraints.
            if (storedPolicyName == null)
            {
                // Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad hoc SAS, and
                // to construct a shared access policy that is saved to the container's shared access policies.
                SharedAccessBlobPolicy adHocPolicy = new SharedAccessBlobPolicy()
                {
                    // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
                    // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                    Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List
                };

                // Generate the shared access signature on the container, setting the constraints directly on the signature.
                sasContainerToken = container.GetSharedAccessSignature(adHocPolicy, null);

                Console.WriteLine("SAS for blob container (ad hoc): {0}", sasContainerToken);
                Console.WriteLine();
            }
            else
            {
                // Generate the shared access signature on the container. In this case, all of the constraints for the
                // shared access signature are specified on the stored access policy, which is provided by name.
                // It is also possible to specify some constraints on an ad hoc SAS and others on the stored access policy.
                sasContainerToken = container.GetSharedAccessSignature(null, storedPolicyName);

                Console.WriteLine("SAS for blob container (stored access policy): {0}", sasContainerToken);
                Console.WriteLine();
            }

            // Return the URI string for the container, including the SAS token.
            return sasContainerToken;
        }

        public async Task<IActionResult> OnPostAsync(string blobName, string blobBody)
        {
            BlobContainerClient bcc = new BlobContainerClient(storageConnectionString, containerName);

            

            // convert string to stream
            byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(blobBody);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

            await bcc.UploadBlobAsync(blobName, stream);
            return Page();
        }
       


    }
}
