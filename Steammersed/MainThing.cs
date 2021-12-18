using Steamworks;
using Steamworks.Data;
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

        public enum LoginStatus {
            LoginSuccessful,
            LoginFailed,
            LoginCancelled,
            SteamGuard
        }

        public LoginStatus Authenticate(string username, string password, string? emailauthcode = "")
        {

            //How to use Steam Guard from a mobile phone (not e-mail)??
            var postDictionary = new Dictionary<string, string>();
            postDictionary.Add("1", "client_id=DE45CD61" +
                    "&grant_type=password&username=" + Uri.EscapeDataString(username) +
                    "&password=" + Uri.EscapeDataString(password) +
                    "&x_emailauthcode=" + emailauthcode +
                    "&scope=read_profile%20write_profile%20read_client%20write_client");


            var response = steamRequestAsync("ISteamUserOAuth/GetTokenWithCredentials/v0001", postDictionary);

            if (response != null)
            {
                JObject data = JObject.Parse(response.Result.ToString());

                if (data["access_token"] != null)
                {
                    access_token = data["access_token"].ToString();
                    if (data["x_steamid"] != null)
                    {
                        steamid = data["x_steamid"].ToString();


                        return login() ? LoginStatus.LoginSuccessful : LoginStatus.LoginFailed;
                    }
                    else return LoginStatus.LoginFailed;

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
        }

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


        private bool login()
        {
            var response = steamRequestAsync("ISteamWebUserPresenceOAuth/Logon/v0001/" +
                "?access_token=" + access_token);

            if (response != null)
            {
                var data = JObject.Parse(response.Result.ToString());

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
            }
            else
            {
                return false;
            }
        }

        public static async Task<string> steamRequestAsync(String get, Dictionary<String, String> post = null)
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

        public SteamApi.root ParseSupportedAPI(String requested_interface, String api_key, String? steamid)
        {
            var content = steamRequestAsync(requested_interface + api_key + "&steamid=" + steamid).GetAwaiter().GetResult();
            SteamApi.root r = null;
            try
            {
                r = JsonConvert.DeserializeObject<SteamApi.root>(content);
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return r;
        }

        public SteamApi.SteamApiGameInfo ParseSupportedGameInfo(string app_id, string api_key, string steam_id)
        {
            var new_content = steamRequestAsync($"ISteamUserStats/GetUserStatsForGame/v2?appid={app_id}&key={api_key}&steamid={steam_id}").GetAwaiter().GetResult();
            //var content = steamRequestAsync("ISteamUserStats/GetUserStatsForGame/v0002/" + "?appid=" + app_id + "&key=" + api_key + "&steamid=" + steam_id).GetAwaiter().GetResult();
            SteamApi.SteamApiGameInfo info = null;
            try
            {
                info = JsonConvert.DeserializeObject<SteamApi.SteamApiGameInfo>(new_content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return info;
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
