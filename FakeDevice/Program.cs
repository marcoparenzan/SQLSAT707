using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Console;

namespace FakeDevice
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = DeviceClient.CreateFromConnectionString(ConfigurationManager.AppSettings["ConnectionString"]);

            var random = new Random();
            while (true)
            {
                var ev = new
                {
                    DeviceId = ConfigurationManager.AppSettings["DeviceId"],
                    Timestamp = DateTimeOffset.Now,
                    Temperature = random.Next(10, 25)
                };
                var json = JsonConvert.SerializeObject(ev);
                var bytes = Encoding.UTF8.GetBytes(json);
                var message = new Message(bytes);

                await client.SendEventAsync(message);
                WriteLine(json);

                await Task.Delay(10000);
            }
        }
    }
}
