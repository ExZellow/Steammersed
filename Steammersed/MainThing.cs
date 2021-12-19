using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamAuth;



namespace Steammersed
{    
    public class MainThing
    {
        /*
        public ServerInfo GetServerInfo()
        {
            String response = steamRequestAsync("ISteamWebAPIUtil/GetServerInfo/v0001").Result.ToString();

            if (response != null)
            {
                JObject data = JObject.Parse(response);

                if (data["servertime"] != null)
                {
                    ServerInfo info = new ServerInfo();
                    info.serverTime = unixTimestamp((long)data["servertime"]);
                    info.serverTimeString = (String)data["servertimestring"];
                    return info;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
       */

        public bool logon(string username, string password)
        { 
            UserLogin login = new UserLogin(username, password);
            LoginResult response = LoginResult.BadCredentials;
            string code = "";
            Console.WriteLine("Start");
            while ((response = login.DoLogin()) != LoginResult.LoginOkay)
            {
                switch (response)
                {
                    case LoginResult.NeedEmail:
                        Console.WriteLine("Please enter your email code: ");
                        code = Console.ReadLine();
                        login.EmailCode = code;
                        break;

                    case LoginResult.Need2FA:
                        Console.WriteLine("2fa");
                        code = Console.ReadLine();
                        login.TwoFactorCode = code;
                        break;

                    default:
                        Console.WriteLine("Smth bad happened");
                        break;
                }
            }
            return true;
        }

        public static async Task<string> steamRequestAsync(string get, Dictionary<string, string> post = null)
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.steampowered.com/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "Steam 1291812 / iPhone");
                if(post == null)
                {
                    HttpResponseMessage response = await client.GetAsync(get);
                    response.EnsureSuccessStatusCode();
                    return response.Content.ReadAsStringAsync().Result.ToString();
                } 
                else
                {
                    var json = new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(get, json);
                    response.EnsureSuccessStatusCode();
                    return response.Content.ReadAsStringAsync().Result.ToString();
                }
            }
        }

        public T DeserializeObject<T>(string request_result)
        {
            T result = default(T);
            try
            {
                result = JsonConvert.DeserializeObject<T>(request_result);
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return result;
        }

        public SteamApi.RootSteamApiList ParseSupportedAPI(string requested_interface, string api_key, string? steamid)
        {
            var request_result = steamRequestAsync($"{requested_interface}/?access_token={api_key}&steamid={steamid}").GetAwaiter().GetResult();
            return DeserializeObject<SteamApi.RootSteamApiList>(request_result);
            
        }

        public SteamApi.SteamApiGameInfo ParseSupportedGameInfo(string requested_interface, string app_id, string api_key, string steam_id)
        {
            var request_result = steamRequestAsync($"{requested_interface}/?appid={app_id}&key={api_key}&steamid={steam_id}").GetAwaiter().GetResult();
            return DeserializeObject<SteamApi.SteamApiGameInfo>(request_result);
        }

        public SteamApi.SteamApiMethod ParseAPIMethod(string requested_interface, string api_key)
        {
            var request_result = steamRequestAsync(requested_interface).GetAwaiter().GetResult();
            return DeserializeObject<SteamApi.SteamApiMethod>(request_result);
        }

        public SteamApi.RootSteamApp ParseAppList(string requested_interface, string api_key)
        {
            var request_result = steamRequestAsync($"{requested_interface}?key={api_key}").GetAwaiter().GetResult();
            return DeserializeObject<SteamApi.RootSteamApp>(request_result);
        }
        private DateTime unixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}
