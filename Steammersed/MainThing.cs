using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Steammersed
{    
    public class MainThing
    {
        private string access_token;
        private string umqid;
        private string steamid;
        private int message = 0;
        /*
        public enum LoginStatus {
            LoginSuccessful,
            LoginFailed,
            LoginCancelled,
            SteamGuard
        }

        public LoginStatus Authenticate(string username, string password, string? emailauthcode = "")
        {

            //How to use Steam Guard from mobile phone (not e-mail)??
            String response = steamRequestAsync("ISteamOAuth2/GetTokenWithCredentials/v0001",
                "client_id=DE45CD61&grant_type=password&username=" + Uri.EscapeDataString(username) + "&password=" + Uri.EscapeDataString(password) + "&x_emailauthcode=" + emailauthcode + "&scope=read_profile%20write_profile%20read_client%20write_client");

            if (response != null)
            {
                JObject data = Newtonsoft.Json.Linq.JObject.Parse(response);

                if (data["access_token"] != null)
                {
                    accessToken = (String)data["access_token"];

                    return login() ? LoginStatus.LoginSuccessful : LoginStatus.LoginFailed;
                }
                else if (((string)data["x_errorcode"]).Equals("steamguard_code_required"))
                    return LoginStatus.SteamGuard;
                else
                    return LoginStatus.LoginFailed;
            }
            else
            {
                return LoginStatus.LoginFailed;
            }
        }

        public LoginStatus Authenticate(String accessToken)
        {
            this.access_token = accessToken;
            return login() ? LoginStatus.LoginSuccessful : LoginStatus.LoginFailed;
        }*/

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
        

        
        private bool login()
        {
            //var response = steamRequestAsync("ISteamWebUserPresenceOAuth/Logon/v0001/" +
              //  "?access_token=" + "E3E5FDDA82E49614AA1D9F65F6C2A8E2");

            //if (response != null)
            //{
                var data = JObject.Parse("ISteamWebUserPresenceOAuth/Logon/v0001/",
                "?access_token=E3E5FDDA82E49614AA1D9F65F6C2A8E2");

                if (data["umqid"] != null)
                {
                    steamid = (String)data["steamid"];
                    umqid = (String)data["umqid"];
                    message = (int)data["message"];
                    return true;
                }
                else
                {
                    return false;
                }
            /*}
            else
            {
                return false;
            }
        }*/

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

        public static async Task<string> steamRequestAsync(String get, String post = null)
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
        /*
        public SteamApi.root GetGlobalStatsForGame(string app_id, string count, string stat_name)
        {
            var content = steamRequestAsync(requested_interface + api_key + "&steamid=" + steamid).GetAwaiter().GetResult();
            SteamApi.root r = null;
            try
            {
                r = JsonConvert.DeserializeObject<SteamApi.root>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return r;
        }
        */


        private DateTime unixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}
