using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroMap.Entity
{
    public class Event
    {
        
        public int Id { get; set; }
        public int User { get; set; }
        public int Inspection { get; set; }
        public string Description { get; set; }
        public string Typeof { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Last_edit_at { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
