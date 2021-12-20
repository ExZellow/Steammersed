using System;
using System.Threading.Tasks;
using Steammersed;
using System.Linq;

namespace SteamLibTest
{
    class Program
    {
        static MainThing process_data = new MainThing();
        static void Main(string[] args)
        {
            var api_key = "E3E5FDDA82E49614AA1D9F65F6C2A8E2";
            var steam_id = "76561198329187801";
            //var app_id = "570";

            var requested_interface =
            //"IPlayerService/GetOwnedGames/v0001/?access_token=";
            "ISteamWebAPIUtil/GetSupportedAPIList/v0001";
            //"ISteamUserStats/GetGlobalStatsForGame/v0001";
            //"ISteamUserStats/GetUserStatsForGame/v2";
            //"IClientStats_1046930/ReportEvent/v1";
            //"ISteamApps/GetAppList/v2";


            //process_data.logon("devienlein", "Qytwer1100");
            var k = new SISteamUserStats(api_key);
            var kk = new SIPlayerService(api_key);
            var xx = kk.GetSteamLevel(76561198329187801).GetAwaiter().GetResult();
            var kek = k.GetUserStatsForGameAsync(76561198329187801, 730).GetAwaiter().GetResult();
            // var k = new SISteamUser(api_key);
            //var kek = k.GetSteamFriendsWithSummaryAsync(76561198329187801).GetAwaiter().GetResult();//k.GetSteamFriendsAsync(76561198329187801).GetAwaiter().GetResult();
            //var kek = process_data.Test().GetAwaiter().GetResult();
            var x = 10;

        }

        public static void GetMethodInfo(string requested_interface, string api_key)
        {
            var method_info = process_data.ParseAPIMethod(requested_interface, api_key);
            Console.WriteLine($"{method_info.name}, {method_info.description}");
        }

        public static void GetGameInfo(string requested_interface, string app_id, string api_key, string steam_id)
        {
            var game_info = process_data.ParseSupportedGameInfo(requested_interface, app_id, api_key, steam_id);
            Console.WriteLine($"{game_info.playerstats.gameName}: {game_info.playerstats.steamID}");
        }

        public static void GetSupportedInterfaces(string requested_interface, string api_key, string steam_id)
        {
            var api_lists = process_data.ParseSupportedAPI(requested_interface, api_key, steam_id);

            foreach (var current_interface in api_lists.apilist.interfaces)
            {
                Console.WriteLine(current_interface.name);
            }

        }

        public static void GetAppList(string requested_interface, string api_key)
        {
            var app_list = process_data.ParseAppList(requested_interface, api_key);
            foreach (var app in app_list.applist.apps)
            {
                Console.WriteLine($"{app.name}: {app.appid}");
            }
        }

    }
}
