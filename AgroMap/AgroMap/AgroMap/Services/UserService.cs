using AgroMap.Entity;
using AgroMap.Resources;
using AgroMap.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AgroMap
{
    class UserService : IService
    {
        private static ISettings AppSettings
        {
            get
            {
                if (CrossSettings.IsSupported)
                    return CrossSettings.Current;

                return null; // or your custom implementation 
            }
        }

        public static async Task<int> Signin(User user)
        {
            HttpResponseMessage response = null;
            try
            {
                HttpClient httpClient = new HttpClient();
                
                httpClient.BaseAddress = new Uri(Strings.ServerURL);
                httpClient.Timeout = TimeSpan.FromSeconds(60);                

                JObject json = new JObject();
                json.Add("email", user.Email);
                json.Add("password", user.Password);
                //var hashAlgorithm = new Blake2B(512);

                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(json.ToString()), "user");


                StringContent content = new StringContent(form.ToString(), Encoding.UTF8, "application/json");


                response = await httpClient.PostAsync(Strings.ServerURISignin, form);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    User __user = JsonConvert.DeserializeObject<User>(responseContent);
                    SaveUserSession(__user);
                    return (int)response.StatusCode;
                }
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine("AGROMAP|UserService.cs|Signin - Timeout: " + ex.Message);
                return 500;
            }
            catch (Exception e)
            {
                Debug.WriteLine("AGROMAP|UserService.cs|Signin: " + e.Message);
            }
            return (int)response.StatusCode;

        }

        public static async Task<int> Signup(User user)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = null;
                httpClient.BaseAddress = new Uri(Strings.ServerURL);


                var json = new JObject();
                json.Add("name", user.Name);
                json.Add("last_name", user.Last_Name);
                json.Add("email", user.Email);
                json.Add("password", user.Password);

                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(json.ToString()), "user");


                StringContent content = new StringContent(form.ToString(), Encoding.UTF8, "application/json");


                response = await httpClient.PostAsync(Strings.ServerURISignup, form);

                return (int)response.StatusCode;

            }
            catch (Exception e)
            {
                Debug.WriteLine("AGROMAP|UserService.cs|Signup: " + e.Message);
                return 404;
            }
        }

        public static Boolean SaveUserSession(User user)
        {
            try
            {
                AppSettings.AddOrUpdateValue("id", user.Id);
                AppSettings.AddOrUpdateValue("name", user.Name);
                AppSettings.AddOrUpdateValue("email", user.Email);
                AppSettings.AddOrUpdateValue("level", user.Level);
                AppSettings.AddOrUpdateValue("password", user.Password);
            }
            catch(Exception e)
            {
                Debug.WriteLine("AGROMAP|UserService.cs|SaveUserSession: " + e.Message);
                return false;
            }

            return true;
        }

        public static User LoadUserSession()
        {
            try
            {
                User u =  new User
                {
                    Id = AppSettings.GetValueOrDefault("id", 0),
                    Name = AppSettings.GetValueOrDefault("name", string.Empty),
                    Email = AppSettings.GetValueOrDefault("email", string.Empty),
                    Level = AppSettings.GetValueOrDefault("level", 3),
                    Password = AppSettings.GetValueOrDefault("password", string.Empty)
                };
                if (u.Id == 0)
                {
                    return null;

                }
                return u;
                
            }
            catch(Exception e)
            {
                Debug.WriteLine("AGROMAP|UserService.cs|LoadUserSession: " + e.Message);
                return null;
            }
            
        }

        public static int GetLoggedUserId()
        {
            try
            {
                return AppSettings.GetValueOrDefault("id", 0);
            }
            catch (Exception e)
            {
                Debug.WriteLine("AGROMAP|UserService.cs|GetLoggedUserId: " + e.Message);
                return 0;
            }

        }

        public static void Logout()
        {
            try
            {
                AppSettings.Remove("id");
                AppSettings.Remove("name");
                AppSettings.Remove("email");
                AppSettings.Remove("level");
                AppSettings.Remove("password");
                
            }
            catch(Exception e)
            {
                Debug.WriteLine("AGROMAP|UserService.cs|Logout: " + e.Message);
            }
        }

    }
}
