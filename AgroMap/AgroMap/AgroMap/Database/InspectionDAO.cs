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
    class InspectionDAO
    {
        private static string Table = "Inspection";
        static readonly SQLiteAsyncConnection db;

        public InspectionDAO()
        {
            try
            {
                if (db == null)
                {
                    string dbpath = DependencyService.Get<IFileHelper>().GetLocalFilePath("agromap_database.db3");
                    db = new SQLiteAsyncConnection(dbpath);
                    db.CreateTableAsync<Inspection>().Wait();
                }
            }
            catch (Exception err)
            {

                Debug.WriteLine("AGROMAP|InspectionDAO(): " + err.Message);
            }
        }

        public async Task<Boolean> Create(Inspection item)
        {
            try
            {
                await db.InsertAsync(item);
                return true;

            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|Insert: " + err.Message);
                return false;
            }
        }

        public async Task<Boolean> Update(Inspection item)
        {
            try
            {
                await db.UpdateAsync(item);
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|Update: " + err.Message);
                return false;
            }
        }

        public async Task<Boolean> Delete(Inspection item)
        {
            try
            {
                await db.DeleteAsync(item);
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|Delete: " + err.Message);
                return false;
            }
        }

        public async  Task<List<Inspection>> GetItens()
        {
            return await db.Table<Inspection>().ToListAsync();
        }

        public async Task<List<Inspection>> GetItensBy(string param, string value, Boolean isInt)
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

        public async Task<Inspection> GetByID(int id)
        {
            try
            {
                return await db.Table<Inspection>().Where(i => i.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|GetByID: " + err.Message);
                return null;
            }
        }

   
    }
}
