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
    public static class EventDAO
    {
        private static string Table = "Event";
        
        public static async Task<List<Event>> GetAll()
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if(db==null)
                return null;
            await CheckTable();
            return await db.Table<Event>().ToListAsync();
        }

        public static async Task<List<Event>> GetEventsByInspection(int inspection)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return null;
            await CheckTable();
            try
            {
                string sql = String.Format("SELECT * FROM {0} WHERE inspection = {1}", Table, inspection);

                return await db.QueryAsync<Event>(sql);
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|GetEventsByInspection: " + err.Message);
                return null;
            }
        }

        public static async Task<List<Event>> GetUnsyncedByInspection(int inspection)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return null;
            await CheckTable();
            try
            {

                string sql = String.Format("SELECT * FROM {0} WHERE inspection = {1} AND synced = 0", Table, inspection);

                return await db.QueryAsync<Event>(sql);
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|GetUnsyncedByInspection: " + err.Message);
                return null;
            }
        }

        public static async Task<Event> GetByID(int id)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return null;
            await CheckTable();

            try
            {
                return await db.Table<Event>().Where(i => i.id == id).FirstOrDefaultAsync();
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|GetByID: " + err.Message);
                return null;
            }
        }

        public static async Task<Boolean> Update(Event item)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            await CheckTable();
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

        public static async Task<Boolean> SetSynced(List<Event> events)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            await CheckTable();
            try
            {
                foreach(Event e in events)
                {
                    e.synced = 1;
                    await db.UpdateAsync(e);
                }
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|SetSynced: " + err.Message);
                return false;
            }
        }

        public static async Task<Boolean> Create(Event item)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            await CheckTable();
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            try
            {
                await db.InsertAsync(item);
                return true;

            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|Create: " + err.Message);
                return false;
            }
        }

        public static async Task<Boolean> Delete(Event item)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            await CheckTable();
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

        public static async Task<Boolean> SaveList(List<Event> events, int inspection)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            await CheckTable();
            try
            {
                //Limpa tabela e armazena apenas o que foi sincronizado com o servidor
                string sql = String.Format("DELETE FROM {0} WHERE inspection = {1}", Table, inspection);
                await db.QueryAsync<Event>(sql);

                // Se lista vazia, retorna
                if (events == null || events.Count == 0)
                    return true;

                foreach (Event e in events)
                {
                    e.synced = 1;
                    await Create(e);
                }
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|SaveInLocalStorage: " + err.Message);
                return false;
            }
        }

        private static async Task<Boolean> CheckTable()
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            try
            {
                await db.Table<Event>().Where(i => i.id == 0).FirstOrDefaultAsync();
            }
            catch
            {
                db.CreateTableAsync<Event>().Wait();
            }
            return true;
        }
        
    }
}
