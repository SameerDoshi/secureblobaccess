using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Storage.Blobs;

namespace StorageAuthDemo
{
    public class DownloadModel : PageModel
    {
        public ActionResult OnGet(string filename)
        {
            string storageConnectionString = "PLACEHOLER"
            string containerName = "data";

            BlobContainerClient bcc = new BlobContainerClient(storageConnectionString, containerName);
            
            Azure.Storage.Blobs.Models.BlobDownloadInfo a = bcc.GetBlobClient(filename).Download();

            long fileByteLength = a.ContentLength;
            byte[] fileContent = new byte[fileByteLength];
            for (int i = 0; i < fileByteLength; i++)
            {
                fileContent[i] = 0x20;
            }
            a.Content.Read(fileContent, 0, (int)fileByteLength);








            return
                File(fileContent, "application/octet-stream",
                        filename);
        }

    

    
    }
}