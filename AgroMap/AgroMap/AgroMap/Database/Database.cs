using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgroMap.Database
{
    static class Database
    {
        private static SQLiteAsyncConnection database;

        public static SQLiteAsyncConnection GetConn()
        {
            try
            {
                if (database == null)
                {
                    string dbpath = DependencyService.Get<IFileHelper>().GetLocalFilePath("agromap_databases.db3");
                    database = new SQLiteAsyncConnection(dbpath);
                }
                return database;
            }catch(Exception e)
            {
                Debug.WriteLine("AGROMAP|Database|GetConn:: " + e.Message);
                return null;
            }
        }

        

    }
}
