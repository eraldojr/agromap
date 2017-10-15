using AgroMap.Entity;
using AgroMap.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgroMap.Database
{
    public static class EventDAO
    {
        private static string Table = "Event";

        public static void DropTable()
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return;
            try
            {
                db.DropTableAsync<Event>().Wait();
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|DropTable: " + err.Message);
                return;
            }
        }

        //Retorna todos eventos de todas as inspeções
        public static async Task<List<Event>> GetAll()
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if(db==null)
                return null;
            await CheckTable();
            return await db.Table<Event>().ToListAsync();
        }

        // Busca todos eventos por inspeção
        public static async Task<List<Event>> GetEventsByInspection(int inspection)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return null;
            await CheckTable();
            try
            {
                string sql = String.Format("SELECT * FROM {0} WHERE inspection = {1}", Table, inspection);
                
                List<Event> list =  await db.QueryAsync<Event>(sql);
                if (list == null)
                    return new List<Event>();
                int list_length = list.Count;
                for (int i = 0; i < list_length; i++)
                {
                    if (list.ElementAt(i).latitude.Equals("delete"))
                    {
                        list.RemoveAt(i);
                        i--;
                        list_length--;

                    }
                }

                return list;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|GetEventsByInspection: " + err.Message);
                return null;
            }
        }

        // Retorna todos eventos marcados como não sincronizados
        // Não sincronizados: synced = 0
        public static async Task<List<Event>> GetUnsyncedByInspection(int inspection)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return null;
            await CheckTable();
            try
            {
                string sql = String.Format("SELECT * FROM {0} WHERE inspection = {1} AND synced = 0", Table, inspection);

                List<Event> list =  await db.QueryAsync<Event>(sql);

                return list;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|GetUnsyncedByInspection: " + err.Message);
                return null;
            }
        }

        // Busca evento pelo id
        public static async Task<Event> GetByID(string id)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return null;
            await CheckTable();

            try
            {
                return await db.Table<Event>().Where(i => i.uuid == id).FirstOrDefaultAsync();
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|GetByID: " + err.Message);
                return null;
            }
        }

        // Método Não utilizado
        // Define eventos como sincronizados
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
                    if (e.latitude.Equals("delete"))
                    {
                        await db.DeleteAsync(e);
                    }
                    else
                    {
                        e.synced = 1;
                        await db.UpdateAsync(e);
                    }
                }
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|SetSynced: " + err.Message);
                return false;
            }
        }

        // Cria ou atualiza um evento
        public static async Task<Boolean> Create(Event item)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            await CheckTable();
            try
            {
                if (item.uuid.Equals(""))
                {
                    item.uuid = GetNewID();
                    await db.InsertAsync(item);
                }
                else
                {
                    await db.UpdateAsync(item);
                }
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|Create: " + err.Message);
                return false;
            }
        }

        // Atualiza um evento
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
                Debug.WriteLine("AGROMAP|EventDAO.cs|Update:: " + err.Message);
                return false;
            }
        }

        public static string GetNewID()
        {
            var uuid = InspectionService.GetDeviceUUID();
            int max_id = Convert.ToInt32(InspectionService.GetMaxID());
            if (uuid.Equals(""))
                return null;
            var new_id = uuid + max_id;
            return new_id;

        }

        // Exclui um evento pelo id
        // Atribui o valor 'delete' em latitude, assim não será mais exibido na lista
        // Atribui synced como 0 para ser enviado ao server
        // No server, com a latitude 'delete' será excluido
        // Após ser enviado, o método setsynced é chamado e o exclui de fato
        public static async Task<Boolean> Delete(Event item)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            await CheckTable();
            try
            {
                item.latitude = "delete";
                item.synced = 0;
                await db.UpdateAsync(item);
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|Delete: " + err.Message);
                return false;
            }
        }

        public static async Task<Boolean> DeleteFromId(string uuid)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
            await CheckTable();
            try
            {
                string sql = String.Format("DELETE FROM {0} WHERE uuid = '{1}'", Table, uuid);
                await db.QueryAsync<Event>(sql);
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|DeleteFromId: " + err.Message);
                return false;
            }
        }

        // Exclui todos eventos de uma inspeção
        public static async Task<Boolean> DeleteFromInspection(int id)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            await CheckTable();
            try
            {
                string sql = String.Format("DELETE FROM {0} WHERE inspection = {1}", Table, id);
                await db.QueryAsync<Event>(sql);
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|Delete: " + err.Message);
                return false;
            }
        }

        // Recebe uma lista de eventos para serem armazenados
        public static async Task<Boolean> SaveList(List<Event> events)
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            await CheckTable();
            try
            {
                // Se lista vazia, retorna
                if (events == null || events.Count == 0)
                    return true;

                foreach (Event e in events)
                {
                    e.synced = 1;
                    await db.InsertAsync(e);
                }
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|EventDAO.cs|SaveList: " + err.Message);
                return false;
            }
        }

        //Verifica se a tabela existe. Se não, cria a tabela
        private static async Task<Boolean> CheckTable()
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            try
            {
                var database = db.Table<Event>();
                await database.CountAsync();
                //await db.Table<Event>().Where(i => i.id == "").FirstOrDefaultAsync();
            }
            catch
            {
                db.CreateTableAsync<Event>().Wait();
            }
            return true;
        }
        
    }
}
