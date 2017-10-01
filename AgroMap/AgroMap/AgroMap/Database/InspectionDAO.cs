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

        // Busca todas inspeções salvas localmente
        public static async Task<List<Inspection>> GetAll()
        {

            SQLiteAsyncConnection db = Database.GetConn();
            if (db != null)
                return await db.Table<Inspection>().ToListAsync();
            Debug.WriteLine("AGROMAP|InspectionDAO.cs|GetLocalItens - Null");
            return null;
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
            await CheckTable();
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
        private static async Task<Boolean> CheckTable()
        {
            SQLiteAsyncConnection db = Database.GetConn();
            if (db == null)
                return false;
            try
            {
                await db.Table<Inspection>().Where(i => i.id == 0).FirstOrDefaultAsync();
            }
            catch
            {
                db.CreateTableAsync<Inspection>().Wait();
            }
            return true;
        }
    }
}
