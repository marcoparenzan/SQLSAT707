using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SQLSATFunction
{

    public static class HandleTemperatureEventParseNet
    {
        [FunctionName("HandleTemperatureEventParseNet")]
        public static void Run([EventHubTrigger("_sqlsat707", Connection = "_iothub")]string myEventHubMessage, TraceWriter log)
        {
            var ev = JsonConvert.DeserializeObject<TemperatureEvent>(myEventHubMessage);

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

                deviceId.Value = ev.DeviceId;
                timestamp.Value = ev.Timestamp;
                temperature.Value = ev.Temperature;
                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }
    }
}
