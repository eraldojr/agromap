using Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroMap

{
    public interface IGetPermission
    {
        bool CheckPermission(string permission);
        bool RequestPermission(string permission);
    }
}