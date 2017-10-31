using Xamarin.Forms;
using AgroMap.Droid;
using Android.Support.V4.App;
using Android.Content.PM;
using System.Diagnostics;
using System;
using Android.App;

[assembly: Dependency(typeof(GetPermissions))]

namespace AgroMap.Droid
{
    class GetPermissions : IGetPermission
    {

        static readonly int REQUEST_PERMISSION_ID = 0;

        public bool CheckPermission(string permission)
        {
            var context = Xamarin.Forms.Forms.Context;
            try
            {
                if (ActivityCompat.CheckSelfPermission(context, permission) != (int)Permission.Granted)
                {
                    return this.RequestPermission(permission);
                }
                return true;
            }
            catch (System.Exception err)
            {
                Debug.WriteLine("AGROMAP|DROID|GetPermission|CheckPermission(): " + err.Message);
                return false;
            }
        }

        public bool RequestPermission(string permission)
        {
            var activity = ((Activity)Xamarin.Forms.Forms.Context);

            

            if (ActivityCompat.ShouldShowRequestPermissionRationale(activity, permission))
            {
                // Permissão autorizada pelo usuário
                ActivityCompat.RequestPermissions(activity, new String[] { permission }, REQUEST_PERMISSION_ID);
                return true;
            }
            else
            {
                // Permissão não autorizada
                ActivityCompat.RequestPermissions(activity, new String[] { permission }, REQUEST_PERMISSION_ID);
                return false;
            }
        }
    }
}