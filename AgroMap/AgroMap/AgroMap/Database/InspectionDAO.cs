using AgroMap.Entity;
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
    static class InspectionDAO
    {
        private static string Table = "Inspection";
        
        public static async  Task<List<Inspection>> GetLocalItens()
        {
            
            SQLiteAsyncConnection db = Database.GetConn();
            if(db!=null)
                return await db.Table<Inspection>().ToListAsync();
            Debug.WriteLine("AGROMAP|InspectionDAO.cs|GetLocalItens - Null");
            return null;
        }

        public static async Task<List<Inspection>> GetLocalItensBy(string param, string value, Boolean isInt)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db != null)
            {
                try
                {
                    int __value = 0;
                    if (isInt)
                    {
                        __value = Convert.ToInt32(value);
                    }
                    string sql = String.Format("SELECT * FROM {0} WHERE {1} = {2}", Table, param, __value);
                    Debug.WriteLine("AGROMAP|EventDAO.cs|GetItensBy: SQL: " + sql);

                    return await db.QueryAsync<Inspection>(sql);
                }
                catch (Exception err)
                {
                    Debug.WriteLine("AGROMAP|EventDAO.cs|GetItensBy: " + err.Message);
                    return null;
                }
            }
            Debug.WriteLine("AGROMAP|InspectionDAO.cs|GetLocalItensBy - Null");
            return null;
        }

        public static async Task<Inspection> GetByID(int id)
        {

            SQLiteAsyncConnection db = Database.GetConn();
            if (db != null)
            {
                try
                {
                    return await db.Table<Inspection>().Where(i => i.id == id).FirstOrDefaultAsync();
                }
                catch (Exception err)
                {
                    Debug.WriteLine("AGROMAP|EventDAO.cs|GetByID: " + err.Message);
                    return null;
                }
            }
            Debug.WriteLine("AGROMAP|EventDAO.cs|GetByID - DBNull");
            return null;
        }

        public static async Task<Boolean> SaveInLocalStorage(List<Inspection> inspections)
        {
            
            SQLiteAsyncConnection db = Database.GetConn();
            if (db != null)
            {
                try
                {
                    
                    db.DropTableAsync<Inspection>().Wait();
                    db.CreateTableAsync<Inspection>().Wait();
                    foreach (Inspection i in inspections)
                    {
                        await db.InsertAsync(i);
                    }
                    return true;
                }
                catch (Exception err)
                {
                    Debug.WriteLine("AGROMAP|InspectionDAO.cs|SaveInLocalStorage: " + err.Message);
                    return false;
                }
            }
            Debug.WriteLine("AGROMAP|InspectionDAO.cs|SaveInLocalStorage - DBNull");
            return false;
        }

        public static async Task<Boolean> Create(Inspection inspection)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db != null)
            {
                try
                {
                    db.CreateTableAsync<Inspection>().Wait();
                    await db.InsertAsync(inspection);
                    return true;
                }
                catch (SQLite.SQLiteException err)
                {
                    Debug.WriteLine("AGROMAP|InspectionDAO.cs|Create|SQLite: " + err.Message);
                }
                catch (Exception err)
                {
                    Debug.WriteLine("AGROMAP|InspectionDAO.cs|Create: " + err.Message);
                    return false;
                }
            }
            Debug.WriteLine("AGROMAP|InspectionDAO.cs|Create - Null");
            return false;
        }

    }
}
