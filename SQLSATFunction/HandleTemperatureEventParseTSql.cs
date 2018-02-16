using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SQLSATFunction
{
    public static class HandleTemperatureEventParseTSql
    {
        [FunctionName("HandleTemperatureEventParseTSql")]
        public static void Run([EventHubTrigger("sqlsat707", Connection = "iothub")]string myEventHubMessage, TraceWriter log)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "ImportTemperatureJSONEvent";
                cmd.CommandType = CommandType.StoredProcedure;

                var json = cmd.Parameters.Add("@Json", SqlDbType.NVarChar, 512);

                json.Value = myEventHubMessage;
                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }
    }
}
