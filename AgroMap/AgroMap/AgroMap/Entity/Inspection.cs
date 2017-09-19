using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroMap.Entity
{
    public class Inspection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public DateTime Start_At { get; set; }
        public DateTime End_At { get; set; }
        public int Supervisor { get; set; }
        public List<int> Members { get; set; }
    }
}
