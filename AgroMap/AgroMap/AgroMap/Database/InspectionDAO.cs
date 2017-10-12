using AgroMap.Entity;
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
    static class InspectionDAO
    {
        private static string Table = "Inspection";

        public static void DropTable()
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return;
            try
            {
                db.DropTableAsync<Inspection>().Wait();
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionDAO.cs|DropTable: " + err.Message);
                return;
            }
        }

        // Busca todas inspeções salvas localmente
        public static async Task<List<Inspection>> GetAll()
        {
            try
            {
            CheckTable();
                SQLiteAsyncConnection db = Database.GetConn();
                if (db != null)
                    return await db.Table<Inspection>().ToListAsync();
            }catch(Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionDAO.cs|GetAll" + err.Message);
            }
            return new List<Inspection>();
            
        }

        // Busca inspecao por ID
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

        // Salva lista de inspeções no armazenamento local
        public static async Task<Boolean> SaveInLocalStorage(List<Inspection> inspections)
        {

            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            CheckTable();
            try
            {
                List<Inspection> local_inspections = await GetAll();
                foreach(Inspection local in local_inspections)
                {
                    var result = inspections.Where(i => i.id == local.id);
                    if(result.Count() == 0)
                    {
                        await EventDAO.DeleteFromInspection(local.id);
                    }
                }
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

        // Salva uma inspeção no armazenamento local
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

        // Exclui inspeção do armazenamento local
        public static async Task<Boolean> Delete(Inspection i)
        {

            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            CheckTable();
            try
            {
                await db.DeleteAsync(i);
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionDAO.cs|Delete: " + err.Message);
                return false;
            }
        }

        //Verifica se a tabela existe. Se não, cria a tabela
        private static Boolean CheckTable()
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            try
            {
                db.Table<Inspection>().Where(i => i.id == 0).FirstOrDefaultAsync();
            }
            catch
            {
                db.CreateTableAsync<Inspection>().Wait();
            }
            return true;
        }
    }
}
