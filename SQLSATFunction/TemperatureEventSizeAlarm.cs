using System;
using System.Data.SqlClient;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Dapper;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SQLSATFunction
{
    public static class TemperatureEventSizeAlarm
    {

        [FunctionName("TemperatureEventSizeAlarm")]
        public static void Run([TimerTrigger("* 0/59 * * * *")]TimerInfo myTimer, [Queue("temperaturealarms", Connection = "storage")] out string message, TraceWriter log)
        {
            message = null;
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var spacesUsed = conn.Query<SpaceUsed>("EXEC sp_spaceused '[dbo].[TemperatureEvents]'");
                foreach (var spaceUsed in spacesUsed)
                {
                    var dataUsed = int.Parse(Regex.Match(spaceUsed.Data, @"\d+").Value);
                    if (dataUsed > 0)
                    {
                        message = JsonConvert.SerializeObject(new
                        {
                            AlarmType = "temperatureeventsspace",
                            DataUsed = dataUsed
                        });
                    }
                }
                conn.Close();
            }
        }
    }
}
