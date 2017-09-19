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
    class EventDAO
    {
        private static string Table = "Event";
        readonly SQLiteAsyncConnection db;

        public EventDAO()
        {
            try
            {
                if (db == null)
                {
                    string dbpath = DependencyService.Get<IFileHelper>().GetLocalFilePath("agromap_database.db3");
                    db = new SQLiteAsyncConnection(dbpath);
                    db.CreateTableAsync<Event>().Wait();
                }
            }
            catch (Exception err)
            {

                Debug.WriteLine("AGROMAP|EventDAO(): " + err.Message);
            }
                
        }
        
        public async  Task<List<Event>> GetItens()
        {
            return await db.Table<Event>().ToListAsync();
        }

        public async Task<List<Event>> GetItensBy(string param, string value, Boolean isInt)
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

                return await db.QueryAsync<Event>(sql);
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|GetItensBy: " + err.Message);
                return null;
            }
        }

        public async Task<Event> GetByID(int id)
        {
            try
            {
                return await db.Table<Event>().Where(i => i.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|GetByID: " + err.Message);
                return null;
            }
        }

        public async Task<Boolean> Update(Event item)
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

        public async Task<Boolean> Insert(Event item)
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

        public async Task<Boolean> Delete(Event item)
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
    }
}
