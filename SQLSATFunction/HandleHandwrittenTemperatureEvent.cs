using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;

namespace SQLSATFunction
{
    public static class HandleHandwrittenTemperatureEvent
    {
        [FunctionName("HandleHandwrittenTemperatureEvent")]
        public static async Task Run([BlobTrigger("handwritten/{name}", Connection = "storage")]string content, string name, TraceWriter log)
        {
            var client = new Microsoft.ProjectOxford.Vision.VisionServiceClient(
                Environment.GetEnvironmentVariable("Computer-Vision-Ocp-Apim-Subscription-Key"),
                Environment.GetEnvironmentVariable("Computer-Vision-Api-Root"));

            HandwritingRecognitionOperationResult result = null;
            try
            {
                var operation = await client.CreateHandwritingRecognitionOperationAsync($"https://sqlsat707.blob.core.windows.net/handwritten/{name}");
                result = await client.GetHandwritingRecognitionOperationResultAsync(operation);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // now insert

            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "ImportTemperatureEvent";
                cmd.CommandType = CommandType.StoredProcedure;

                var deviceId = cmd.Parameters.Add("@DeviceId", SqlDbType.NVarChar, 32);
                var timestamp = cmd.Parameters.Add("@Timestamp", SqlDbType.DateTimeOffset);
                var temperature = cmd.Parameters.Add("@Temperature", SqlDbType.Float);

                foreach (var line in result.RecognitionResult.Lines)
                {
                    deviceId.Value = line.Words[0].Text;
                    timestamp.Value = DateTimeOffset.Now;
                    temperature.Value = double.Parse(line.Words[1].Text);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }
    }
}
