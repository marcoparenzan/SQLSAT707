using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SQLSATFunction
{
    public static class ImportHandwrittenTemperatureEvent
    {
        [FunctionName("ImportHandwrittenTemperatureEvent")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, [Blob("handwritten/{rand-guid}", Connection = "storage")] CloudBlockBlob blob, TraceWriter log)
        {
            var multipart = await req.Content.ReadAsMultipartAsync();

            var bytes = await multipart.Contents[0].ReadAsByteArrayAsync();
            await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
