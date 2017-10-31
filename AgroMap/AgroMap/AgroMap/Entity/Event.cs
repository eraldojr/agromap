using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroMap.Entity
{
    public class Event
    {
        [SQLite.PrimaryKey]
        public string uuid { get; set; }
        public int user { get; set; }
        public int inspection { get; set; }
        public string description { get; set; }
        public string kind { get; set; }
        public DateTime last_edit_at { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
}
