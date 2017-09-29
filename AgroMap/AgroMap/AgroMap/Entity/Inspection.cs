using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroMap.Entity
{
    public class Inspection
    {
        public int id { get; set; }
        public string name { get; set; }
        public string created_at { get; set; }
        public DateTime start_at { get; set; }
        public DateTime end_at { get; set; }
        public int supervisor { get; set; }
    }
}
