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

        /*
        private bool login()
        {
            String response = steamRequestAsync("ISteamWebUserPresenceOAuth/Logon/v0001",
                "?access_token=" + accessToken);

            if (response != null)
            {
                JObject data = JObject.Parse(response);

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
        }*/

        private async Task<string> steamRequestAsync(String get, Dictionary<String, String> post = null)
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
                return "";
            }
        }

        public SteamApi.root ParseSupportedAPI()
        {
            var content = steamRequestAsync("ISteamWebAPIUtil/GetSupportedAPIList/v0001/").GetAwaiter().GetResult();
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

        private DateTime unixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}
