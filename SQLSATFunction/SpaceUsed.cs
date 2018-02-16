using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLSATFunction
{
    public class SpaceUsed
    {
        public string Name { get; set; }
        public string Rows { get; set; }
        public string Reserved { get; set; }
        public string Data { get; set; }
        public string Index_Size { get; set; }
        public string Unused { get; set; }
    }
}
