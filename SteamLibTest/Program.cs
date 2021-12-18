using System;
using System.Threading.Tasks;
using Steammersed;

namespace SteamLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = new MainThing();


            var requested_interface =
            //"IPlayerService/GetOwnedGames/v0001/?access_token=";
            //"ISteamWebAPIUtil/GetSupportedAPIList/v0001/?access_token=";
            //"ISteamUserStats/GetGlobalStatsForGame/v0001/?access_token=";
            "ISteamUserStats/GetUserStatsForGame/v0002/";
            var app_id = "570";
            var api_key = "E3E5FDDA82E49614AA1D9F65F6C2A8E2";
            var steam_id = "76561198329187801";

            //MainThing.LoginStatus loginStatus = x.Authenticate("green_underdevil", "AllMight_OneForAll");

            //Console.WriteLine(loginStatus);

            //var k = x.ParseSupportedAPI(requested_interface, api_key, steamid);
            var k = x.ParseSupportedGameInfo(app_id,
                                             api_key,
                                             steam_id);

            //foreach (var current_info in k.playerstats)
            //{
                Console.WriteLine(k.playerstats.gameName + " " + k.playerstats.steamID);
            //}


                /*foreach (var current_interface in k.apilist)
                {
                    Console.WriteLine(current_interface.name);
                }*/


            }
        }
}
