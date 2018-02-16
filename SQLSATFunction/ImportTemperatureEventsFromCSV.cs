using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace SQLSATFunction
{
    public static class ImportTemperatureEventsFromCSV
    {
        [FunctionName("ImportTemperatureEventsFromCSV")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var multipart = await req.Content.ReadAsMultipartAsync();

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

                foreach (var content in multipart.Contents)
                {
                    var streamCSV = await content.ReadAsStreamAsync();
                    using (var reader = new StreamReader(streamCSV))
                    {
                        reader.ReadLine();
                        var line = reader.ReadLine();
                        while (!string.IsNullOrWhiteSpace(line))
                        {
                            var cols = line.Split(';');

                            deviceId.Value = cols[0];
                            timestamp.Value = DateTimeOffset.Parse(cols[1]);
                            temperature.Value = double.Parse(cols[2]);
                            cmd.ExecuteNonQuery();

                            line = reader.ReadLine();
                        }
                    }
                }

                conn.Close();
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
