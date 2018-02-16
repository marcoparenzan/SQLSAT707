using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLSATFunction
{
    public class TemperatureEvent
    {
        public int? Id { get; set; }
        public string DeviceId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public double Temperature { get; set; }
    }
}
