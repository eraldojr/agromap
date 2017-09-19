using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroMap.Entity
{
    class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Last_Name { get; set; }

        public string Email { get; set; }

        public int Level { get; set; }

        public string Password { get; set; }

        public string Created_at { get; set; }
    }
}
