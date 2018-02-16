using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Dapper;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace SQLSATFunction
{
    public static class ExportTemperatureEventsInCSV
    {
        [FunctionName("ExportTemperatureEventsInCSV")]
        public static async Task Run([TimerTrigger("* 0/59 * * * *")]TimerInfo myTimer, [Blob("temperatureevents", Connection="storage")] CloudBlobContainer eventBlobs, TraceWriter log)
        {
            await eventBlobs.CreateIfNotExistsAsync();

            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var events = conn.Query<TemperatureEvent>("SELECT * FROM TemperatureEvents"); // WHERE...
                foreach (var ev in events)
                {
                    var blob = eventBlobs.GetBlockBlobReference($"{ev.Id}");
                    await blob.UploadTextAsync(JsonConvert.SerializeObject(ev));
                }

                // conn.Execute("DELETE * FROM TemperatureEvents"); // WHERE...
                conn.Close();
            }
        }
    }
}
