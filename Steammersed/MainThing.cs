using Steamworks;
using Steamworks.Data;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

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

        private async Task<string> steamRequestAsync(String get, String post = null)
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.steampowered.com/" + get);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                //client.DefaultRequestHeaders.Add("Accept-Language", "en-us");
                client.DefaultRequestHeaders.Add("User-Agent", "Steam 1291812 / iPhone");

                HttpResponseMessage response = await client.GetAsync("ISteamWebAPIUtil/GetSupportedAPIList/v0001/");
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result.ToString();



                if (post != null)
                {
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                    client.DefaultRequestHeaders.Add("Accept-Language", "en-us");
                    client.DefaultRequestHeaders.Add("User-Agent", "Steam 1291812 / iPhone");

                    //client.PostAsync()
                    
                    //byte[] postBytes = Encoding.ASCII.GetBytes(post);
                    //request.ContentType = "application/x-www-form-urlencoded";
                    //request.ContentLength = postBytes.Length;

                    //Stream requestStream = request.GetRequestStream();
                    //requestStream.Write(postBytes, 0, postBytes.Length);
                    //requestStream.Close();

                    //message++;
                }

                try
                {
                    //HttpWebResponse responseBody = (HttpWebResponse)request.GetResponse();
                    if ((int)response.StatusCode != 200) return null;

                    //String src = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    //response.Close();
                    //return src;
                }
                catch (WebException e)
                {
                    return null;
                }
            }
        }

        private DateTime unixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }




    }
}
