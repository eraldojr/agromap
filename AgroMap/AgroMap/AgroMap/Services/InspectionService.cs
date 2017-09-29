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

        // Sincroniza as inspeções e eventos com o server
        // 1 - Busca todas as inspeções do server
        // 2 - Envia eventos não sincronizados para o server
        // 3 - Obtém os eventos do server e salva no armazenamento local
        public static async Task<Boolean> SyncWithServer()
        {
            try
            {
                var __inspections = await GetInspectionsFromServer(); // 1 - Obtém as inspeções
                if (__inspections.Count == 0 || __inspections == null)
                    return false;
                List<Inspection> inspections = __inspections;
                if (!await InspectionDAO.SaveInLocalStorage(inspections)) // Salva as inspeções no armazenamento local
                    return false;

                foreach(Inspection i in inspections)
                {
                    // 2 - Obtém lista de eventos não sincronizados, para que possam ser enviados
                    List<Event> events = await EventDAO.GetUnsyncedByInspection(i.id);
                    {
                        if (!await SendEvents(events)) // Envia lista
                            return false;

                        // 3 - Busca todos eventos do servidor e atualiza armazenamento local
                        events = await GetEventsByInspection(i.id);
                        await EventDAO.SaveList(events, i.id); // Salva no armazenamento local
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|SyncWithServer: " + e.Message);
                return false;
            }
        }

        // Obtém lista de inspeções do servidor
        public async static Task<List<Inspection>> GetInspectionsFromServer()
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
                    if (responseContent.ToString().Equals("None"))
                        return new List<Inspection>();
                    return JsonConvert.DeserializeObject<List<Inspection>>(responseContent);
                }
            }catch(Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|GetInspectionsFromServer:: " + err);
            }
            return null;
        }

        // Busca eventos por inspeção do servidor
        public async static Task<List<Event>> GetEventsByInspection(int Id)
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
                Debug.WriteLine("AGROMAP|UserService.cs|EventsByInspection: " + e.Message);
                return null;
            }

        }

        // Envia inspeção nova para o servidor
        public async static Task<Boolean> CreateInspection(Inspection i)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = null;
            httpClient.BaseAddress = new Uri(Strings.ServerURL);
            var json = new JObject();
            try
            {
                if (i.id > 0)
                {
                    json.Add("id", i.id);
                    json.Add("name", i.name);
                    json.Add("start_at", i.start_at);
                    json.Add("end_at", i.end_at);
                    json.Add("supervisor", UserService.GetLoggedUserId());
                    int[] members = { 1 };
                    json.Add("members", 1);


                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(new StringContent(json.ToString()), "inspection");

                    StringContent content = new StringContent(form.ToString(), Encoding.UTF8, "application/json");

                    response = await httpClient.PostAsync(Strings.ServerURIUpdateInspection, form);

                    return response.IsSuccessStatusCode;
                }
                else
                {
                    json.Add("id", i.id);
                    json.Add("name", i.name);
                    json.Add("start_at", i.start_at);
                    json.Add("end_at", i.end_at);
                    json.Add("supervisor", UserService.GetLoggedUserId());
                    int[] members = {1};
                    var array = JArray.FromObject(members);
                    json.Add("members", array);

                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(new StringContent(json.ToString()), "inspection");

                    StringContent content = new StringContent(form.ToString(), Encoding.UTF8, "application/json");

                    response = await httpClient.PostAsync(Strings.ServerURICreateInspection, form);

                    if (response.IsSuccessStatusCode)
                    {

                        return true;
                    }
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return false;
                    
                }
            }catch(Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|Create: " + err.Message);
                return false;
            }
        }

        // Envia evento novo para o servidor
        public async static Task<Boolean> SendEvent(Event e)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = null;
            httpClient.BaseAddress = new Uri(Strings.ServerURL);
            var json = new JObject();
            try
            {
                if (e.id > 0)
                {
                    json.Add("user", UserService.GetLoggedUserId());
                    json.Add("id", e.id);
                    json.Add("inspection", e.inspection);
                    json.Add("typeof", e.types);
                    json.Add("description", e.description);
                    json.Add("latitude", e.latitude);
                    json.Add("longitude", e.longitude);


                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(new StringContent(json.ToString()), "event");

                    StringContent content = new StringContent(form.ToString(), Encoding.UTF8, "application/json");

                    response = await httpClient.PostAsync(Strings.ServerURICreateEvent, form);

                    return response.IsSuccessStatusCode;
                }
                else
                {
                    json.Add("user", UserService.GetLoggedUserId());
                    json.Add("id", e.id);
                    json.Add("inspection", e.inspection);
                    json.Add("typeof", Convert.ToInt32(e.types));
                    json.Add("description", e.description);
                    json.Add("latitude", e.latitude);
                    json.Add("longitude", e.longitude);

                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(new StringContent(json.ToString()), "event");

                    StringContent content = new StringContent(form.ToString(), Encoding.UTF8, "application/json");

                    response = await httpClient.PostAsync(Strings.ServerURICreateEvent, form);

                    if (response.IsSuccessStatusCode)
                    {

                        return true;
                    }
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("AGROMAP|InspectionService.cs|>>: " + responseContent);
                    return false;

                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|CreateEvent: " + err.Message);
                return false;
            }
        }

        // Envia lista de eventos para o servidor
        public async static Task<Boolean> SendEvents(List<Event> events)
        {
            if (events.Count == 0)
                return true;
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = null;
            httpClient.BaseAddress = new Uri(Strings.ServerURL);
            try
            {
                var json_list = JsonConvert.SerializeObject(events);
                    
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(json_list.ToString()), "event"); 

                StringContent content = new StringContent(form.ToString(), Encoding.UTF8, "application/json");

                response = await httpClient.PostAsync(Strings.ServerURICreateEvents, form);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                var responseContent = await response.Content.ReadAsStringAsync();
                return false;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|CreateEvent: " + err.Message);
                return false;
            }
        }

    }
}
