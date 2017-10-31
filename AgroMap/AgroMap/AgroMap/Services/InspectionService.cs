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
using System.Threading;

namespace AgroMap.Services
{
    class InspectionService
    {
        private static int timeout_seconds = 60;
        private static HttpClient httpClient;

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
        // 2 - Envia eventos não sincronizados para o server e faz upload de fotos
        // 3 - Exclui os eventos do armazenamento local
        public static async Task<Boolean> SyncWithServer(CancellationToken token)
        {
            try
            {
                // Token de cancelamento de tarefa
                if (token.IsCancellationRequested)
                    return false;

                // 1 - Obtém as inspeções
                List<Inspection> inspections = await GetInspectionsFromServer(); 
                if (inspections == null)
                {
                    Debug.WriteLine("AGROMAP|InspectionService.cs|SyncWithServer - Inspections Null");
                    return false;
                }

                // Token de cancelamento de tarefa
                if (token.IsCancellationRequested)
                    return false;

                // Salva as inspeções no armazenamento local{
                if (!await InspectionDAO.SaveInLocalStorage(inspections))
                {
                    Debug.WriteLine("AGROMAP|InspectionService.cs|SyncWithServer - Erro ao salvar inspecoes");
                    return false;
                }

                // Token de cancelamento de tarefa
                if (token.IsCancellationRequested)
                    return false;

                List<Event> events = new List<Event>();
                foreach (Inspection i in inspections)
                {
                    // 2 - Obtém lista de eventos para que possam ser enviados
                    events = await EventDAO.GetEventsByInspection(i.id);
                    {
                        // Token de cancelamento de tarefa
                        if (token.IsCancellationRequested)
                            return false;

                        // Envia lista e faz upload de fotos
                        if (!await SendEvents(events))
                        {
                            Debug.WriteLine("AGROMAP|InspectionService.cs|SyncWithServer - Erro ao enviar eventos");
                            return false;
                        }
                        
                        // Token de cancelamento de tarefa
                        if (token.IsCancellationRequested)
                            return false;

                        // 3 - Exclui todos os eventos do armazenamento local
                        await EventDAO.DeleteFromInspection(i.id);
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
                HttpResponseMessage response = null;
                GetHttpClient();

                response = await httpClient.GetAsync(Strings.ServerURIInspectionAll);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        return JsonConvert.DeserializeObject<List<Inspection>>(responseContent);
                    }
                    catch
                    {
                        return new List<Inspection>();
                    }   
                }
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|GetInspectionsFromServer - Timeout: " + ex.Message);
                return null;
            }
            catch (Exception err)
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
                HttpResponseMessage response = null;
                HttpClient httpClient = GetHttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(timeout_seconds);

                response = await httpClient.GetAsync(Strings.ServerURIEventByInspection + Id.ToString());

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        return JsonConvert.DeserializeObject<List<Event>>(responseContent);

                    }
                    catch (Exception)
                    {
                        return new List<Event>();             
                    }
                    

                }
                return null;
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|GetEventsByInspection - Timeout: " + ex.Message);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|GetEventsByInspection: " + e.Message);
                return null;
            }

        }

        // Envia inspeção nova para o servidor
        public async static Task<Boolean> CreateInspection(Inspection i)
        {
            HttpResponseMessage response = null;
            HttpClient httpClient = GetHttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(timeout_seconds);

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
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|CreateInspection - Timeout: " + ex.Message);
                return false;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|Create: " + err.Message);
                return false;
            }
        }

        // Envia solicitação para excluir inspeção, exclui inspeção e eventos do armazenamento local
        public async static Task<Boolean> DeleteInspection(Inspection i)
        {
            HttpResponseMessage response = null;
            GetHttpClient();

            var json = new JObject();
            try
            {
                json.Add("id", i.id);

                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(json.ToString()), "inspection");

                StringContent content = new StringContent(form.ToString(), Encoding.UTF8, "application/json");

                response = await httpClient.PostAsync(Strings.ServerURIDeleteInspection, form);

                if (response.IsSuccessStatusCode)
                {
                    await InspectionDAO.Delete(i);
                    await EventDAO.DeleteFromInspection(i.id);
                    return true;
                }
                return false;
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|DeleteInspection - Timeout: " + ex.Message);
                return false;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|Delete: " + err.Message);
                return false;
            }
        }

        // Envia evento novo para o servidor

        //public async static Task<Boolean> SendEvent(Event e)
        //{
        //    HttpResponseMessage response = null;
        //    GetHttpClient();

        //    var json = new JObject();
        //    try
        //    {
        //        json.Add("user", UserService.GetLoggedUserId());
        //        json.Add("id", e.uuid);
        //        json.Add("inspection", e.inspection);
        //        json.Add("kind", e.kind);
        //        json.Add("description", e.description);
        //        json.Add("latitude", e.latitude);
        //        json.Add("longitude", e.longitude);

        //        MultipartFormDataContent form = new MultipartFormDataContent();
        //        form.Add(new StringContent(json.ToString()), "event");

        //        StringContent content = new StringContent(form.ToString(), Encoding.UTF8, "application/json");

        //        response = await httpClient.PostAsync(Strings.ServerURICreateEvent, form);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            PhotoService.UploadFile(e.uuid);
        //            return true;
        //        }
        //        //var responseContent = await response.Content.ReadAsStringAsync();
        //        return false;
        //    }
        //    catch (TaskCanceledException ex)
        //    {
        //        Debug.WriteLine("AGROMAP|InspectionService.cs|SendEvent - Timeout: " + ex.Message);
        //        return false;
        //    }
        //    catch (Exception err)
        //    {
        //        Debug.WriteLine("AGROMAP|InspectionService.cs|CreateEvent: " + err.Message);
        //        return false;
        //    }
        //}

        // Envia lista de eventos para o servidor

        public async static Task<Boolean> SendEvents(List<Event> events)
        {
            if (events.Count == 0)
                return true;
            GetHttpClient();
            HttpResponseMessage response = null;

            try
            {
                //Serializa eventos em formato JSON
                var json_list = JsonConvert.SerializeObject(events);
                    
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(json_list.ToString()), "event"); 

                StringContent content = new StringContent(form.ToString(), Encoding.UTF8, "application/json");

                response = await httpClient.PostAsync(Strings.ServerURICreateEvents, form);

                if (response.IsSuccessStatusCode)
                {
                    // Se retornar sucesso, os eventos foram enviados
                    // Então começa a enviar as fotos
                    if( await UploadPhotos(events))
                        return true;
                }
                
                return false;
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|SendEvents - Timeout: " + ex.Message);
                return false;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|CreateEvent: " + err.Message);
                return false;
            }
        }

        // Envia as fotos
        private static async Task<bool> UploadPhotos(List<Event> events)
        {
            int max = 0; // Índice da última foto enviada
            try
            {
                // Para cada evento, envia sua foto
                for (int i = 0; i < events.Count; i++)
                {
                    if (!await PhotoService.UploadFile(events[i].uuid))
                        return false;
                    max = i;
                }
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|UploadPhotos(): " + err.Message);
                
                // Se chegou aqui, aconteceu um erro, ou foi cancelado pelo usuário
                // Faz um loop até o índice que foi enviado com sucesso
                // e exclui os que já foram enviados
                for (int i = 0; i < events.Count; i++)
                {
                    EventDAO.Delete(events[i].uuid);
                }
                return false;
            }
        }

        // Solicita ao server o UUID que será utilizado para compor as PKs dos eventos
        // Armazena em 'Shared Preferences'
        public static async Task<Boolean> SetDeviceUUID()
        {
            if (!GetDeviceUUID().Equals("") && GetDeviceUUID() != null)
                return true;
            GetHttpClient();
            
            string uuid = "";
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(Strings.ServerURIGetUUID);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = (JObject)JsonConvert.DeserializeObject(responseContent);
                    uuid = data["UUID"].Value<string>();
                    AppSettings.AddOrUpdateValue("max_id", 0);
                    AppSettings.AddOrUpdateValue("uuid", uuid);
                    return true;
                }   
            }
            catch (Exception e)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|SetDeviceUUID: " + e.Message);
            }
            return false;
            
        }

        public static string GetDeviceUUID()
        {
            try
            {
                return AppSettings.GetValueOrDefault("uuid", "");
            }
            catch (Exception e)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|GetDeviceUUID: " + e.Message);
                return null;
            }

        }

        // Retorna novo id de evento
        public static int GetMaxID()
        {
            try
            {
                int id = AppSettings.GetValueOrDefault("max_id", 0);
                AppSettings.AddOrUpdateValue("max_id", id + 1);
                return id;
            }
            catch (Exception e)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|GetMaxID: " + e.Message);
                return 0;
            }

        }

        // Retorna novo id de evento, a diferença é que este não incrementa o valor do último ID salvo
        // Ou seja, o método anterior retorna o novo ID e já o salva como último ID, este apenas retorna
        public static string GetNextID()
        {
            try
            {
                int id = AppSettings.GetValueOrDefault("max_id", 0);
                return GetDeviceUUID() + id.ToString();
            }
            catch (Exception e)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|GetNextID: " + e.Message);
                return "";
            }

        }

        // Retorna id do ultimo evento criado
        public static string GetLastID()
        {
            try
            {
                int id = AppSettings.GetValueOrDefault("max_id", 0);
                return GetDeviceUUID() + (id - 1).ToString();
            }
            catch (Exception e)
            {
                Debug.WriteLine("AGROMAP|InspectionService.cs|GetLastID: " + e.Message);
                return "";
            }

        }        

        // Retorna objeto http para conexão
        private static HttpClient GetHttpClient()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Strings.ServerURL);
            httpClient.Timeout = TimeSpan.FromSeconds(timeout_seconds);

            return httpClient;
        }

        // Método chamado a partir da tela InspectionScreen, para cancelar sincronização
        public static void DisposeHTTPCLient()
        {
            httpClient.CancelPendingRequests();
            httpClient.Dispose();
        }

    }
}

