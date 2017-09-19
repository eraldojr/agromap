using AgroMap.Entity;
using AgroMap.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using AgroMap.Database;

namespace AgroMap.Services
{
    class InspectionService
    {
        private static ISettings AppSettings
        {
            get
            {
                if (CrossSettings.IsSupported)
                    return CrossSettings.Current;

                return null;
            }
        }

        public static async Task<List<Inspection>> GetAvailable()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = null;
                httpClient.BaseAddress = new Uri(Strings.ServerURL);

                response = await httpClient.GetAsync(Strings.ServerURIInspectionAll);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    List<Inspection> data = JsonConvert.DeserializeObject<List<Inspection>>(responseContent);
                    
                    return data;
                    
                }
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("AGROMAP|UserService.cs|GetAvailable: " + e.Message);
                return null;
            }

        }

        public async static Task<List<Event>> EventsByInspection(int Id)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = null;
                httpClient.BaseAddress = new Uri(Strings.ServerURL);

                response = await httpClient.GetAsync(Strings.ServerURIEventByInspection + Id.ToString());

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    List<Event> data = JsonConvert.DeserializeObject<List<Event>>(responseContent);
                    return data;

                }
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("AGROMAP|UserService.cs|GetAvailable: " + e.Message);
                return null;
            }

        }

        public async static Task<Boolean> Create(Inspection i)
        {
            try
            {
                if (i.Id > 0)
                {
                    //Atualiza existente
                    InspectionDAO dao = new InspectionDAO();
                    return await dao.Create(i);
                }
                else
                {
                    //Cria nova inspeção
                    InspectionDAO dao = new InspectionDAO();
                    return await dao.Update(i);

                }
            }catch(Exception err)
            {
                Debug.WriteLine("AGROMAP|UserService.cs|GetAvailable: " + err.Message);
                return false;
            }
        }
    }
}
