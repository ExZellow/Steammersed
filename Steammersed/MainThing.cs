using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Steammersed
{
    internal class UnixTimeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            ulong timestamp = ulong.Parse(reader.Value.ToString());
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
        public override bool CanWrite => false;
        public override bool CanConvert(Type objectType)
        {
            return typeof(long).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

    }

    public class SSteamRequestParameter
    {
        public string m_name { get; private set; }
        public string m_value { get; private set; }
        public SSteamRequestParameter(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            m_name = name;
            m_value = value;
        }
        public override string ToString()
        {
            return $"{m_name}={m_value}";
        }
    }
    public class SSteamPureRequest
    {
        private string m_web_api_key;
        private HttpClient m_client;
        private string m_base_uri = "https://api.steampowered.com/";
        public SSteamPureRequest(string web_api_key, HttpClient client)
        {

            m_web_api_key = web_api_key;
            m_client = client ?? new HttpClient();
        }

        public async Task<T?> GetAsync<T>(string interface_name, string method_name, int version, List<SSteamRequestParameter> args)
        {
            if (m_client == null)
                m_client = new HttpClient();
            return await MakeRequest<T>(HttpMethod.Get, interface_name, method_name, version, args);
        }
        public async Task<T?> PostAsync<T>(string interface_name, string method_name, int version, List<SSteamRequestParameter> args)
        {
            return await MakeRequest<T>(HttpMethod.Post, interface_name, method_name, version, args);
        }
        public async Task<T?> MakeRequest<T>(HttpMethod method, string interface_name, string method_name, int version, List<SSteamRequestParameter> args)
        {
            if (args == null)
                args = new List<SSteamRequestParameter>();
            args.Insert(0, new SSteamRequestParameter("key", m_web_api_key));
            string query = $"{interface_name}/{method_name}/v{version}/?";
            try
            {
                if (args.Count > 0)
                    query += $"{string.Join("&", args)}";
                HttpResponseMessage response = null;
                m_client.BaseAddress = new Uri(m_base_uri);
                //m_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (HttpMethod.Get == method)
                {
                    response = await m_client.GetAsync(query);
                    response.EnsureSuccessStatusCode();
                    T? ret = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result.ToString());
                    return ret;
                }
                else if (HttpMethod.Post == method)
                {
                    FormUrlEncodedContent content = new FormUrlEncodedContent(args.ToDictionary(p => p.m_name, p => p.m_value));
                    response = await m_client.PostAsync(query, content).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    T? ret = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result.ToString());
                    return ret;
                }
            }
            catch (Exception ex)
            {

            }
            return default;
        }
    }
    public class UserSummary
    {
        public string personaname { get; set; }
        public string steamid { get; set; }
        public string realname { get; set; }
        public string loccountrycode { get; set; }
    }

    public class UserSummaryList
    {
        public List<UserSummary> players { get; set; }
    }


    public class UserSummaryRoot
    {
        public UserSummaryList response { get; set; }
    }

    public class friend
    {
        public string steamid { get; set; }
        public string relationship { get; set; }

        [JsonConverter(typeof(UnixTimeConverter))]
        //[JsonProperty("friend_since")]
        public DateTime friend_since { get; set; }
    }
    public class SteamFriendList
    {

        public List<friend> friends { get; set; }
    }
    public class SteamFriendRoot
    {
        public SteamFriendList friendslist { get; set; }
    }
    public class UserStat
    {
        public string name { get; set; }

        public double value { get; set; }
    }

    public class UserStatAchievement
    {
        public string name { get; set; }

        public uint achieved { get; set; }
    }

    public class UserStatsForGameResult
    {
        public string steamID { get; set; }

        public string gameName { get; set; }

        public List<UserStat> stats { get; set; }

        public List<UserStatAchievement> achievements { get; set; }
    }

    public class UserStatsForGameRoot
    {
        public UserStatsForGameResult playerstats { get; set; }
    }
    public class SISteamUserStats
    {
        private string m_web_api_key;
        private static readonly string m_interface = "ISteamUserStats";
        public SISteamUserStats(string web_api_key) => m_web_api_key = web_api_key;

        public async Task<UserStatsForGameResult> GetUserStatsForGameAsync(ulong steamid, uint appid)
        {
            var web_request = new SSteamPureRequest(m_web_api_key, new HttpClient());
            var args = new List<SSteamRequestParameter>();
            args.Add(new SSteamRequestParameter("steamid", $"{steamid}"));
            args.Add(new SSteamRequestParameter("appid", $"{appid}"));
            var list = await web_request.GetAsync<UserStatsForGameRoot>(m_interface, "GetUserStatsForGame", 2, args);
            return list.playerstats ?? default; //refactor this
        }
    }
    public class UserLevel {
        public int player_level { get; set; }
    }
    public class UserSteamLevelRoot
    {
        public UserLevel response { get; set; }
    }
    public class SIPlayerService
    {
        private string m_web_api_key;
        private static readonly string m_interface = "IPlayerService";
        public SIPlayerService(string web_api_key) => m_web_api_key = web_api_key;

        public async Task<UserLevel> GetSteamLevel(ulong steamid)
        {
            var web_request = new SSteamPureRequest(m_web_api_key, new HttpClient());
            var args = new List<SSteamRequestParameter>();
            args.Add(new SSteamRequestParameter("steamid", $"{steamid}"));
            var list = await web_request.GetAsync<UserSteamLevelRoot>(m_interface, "GetSteamLevel", 1, args);
            return list.response ?? default; //refactor this
        }
    }
    public class SISteamUser
    {
        private string m_web_api_key;
        private static readonly string m_interface = "ISteamUser";
        public SISteamUser(string web_api_key) => m_web_api_key = web_api_key;
        public async Task<List<UserSummary>> GetSteamFriendsWithSummaryAsync(ulong steamid)
        {
            var web_request = new SSteamPureRequest(m_web_api_key, new HttpClient());
            var args = new List<SSteamRequestParameter>();
            args.Add(new SSteamRequestParameter("steamid", $"{steamid}"));
            var list = await GetSteamFriendsAsync(steamid);
            if(list != null && list.Count > 0)
            {
                string steamids = list.Select(i => i.steamid).Aggregate((i, j) => i + "," + j);
                args.Clear();
                args.Add(new SSteamRequestParameter("steamids", steamids));
                web_request = new SSteamPureRequest(m_web_api_key, new HttpClient());
                var ret = await web_request.GetAsync<UserSummaryRoot>(m_interface, "GetPlayerSummaries", 2, args);
                return ret?.response.players ?? default; //refactor this
            }
            return default;
        }
        public async Task<List<friend>> GetSteamFriendsAsync(ulong steamid)
        {
            var args = new List<SSteamRequestParameter>();
            args.Add(new SSteamRequestParameter("steamid", $"{steamid}"));
            var web_request = new SSteamPureRequest(m_web_api_key, new HttpClient());
            var ret = await web_request.GetAsync<SteamFriendRoot>(m_interface, "GetFriendList", 1, args);
            return ret?.friendslist.friends ?? default; //refactor this
        }
    }
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
        public static async Task<string> steamRequestAsync(string get, Dictionary<string, string> post = null)
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            //https://partner.steam-api.com/IInventoryService/GetInventory/v1/?key=E3E5FDDA82E49614AA1D9F65F6C2A8E2&appid=730&steamid=76561198329187801
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
            //var x = logon("narik228891", "readifgay1337");
            var request_result = steamRequestAsync($"{requested_interface}/").GetAwaiter().GetResult();
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
    }
}
