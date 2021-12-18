using System;
using System.Threading.Tasks;
using Steammersed;
using System.Linq;

namespace SteamLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var process_data = new MainThing();

            var api_key = "E3E5FDDA82E49614AA1D9F65F6C2A8E2";
            var steam_id = "76561198329187801";
            var app_id = "570";

            var requested_interface =
            //"IPlayerService/GetOwnedGames/v0001/?access_token=";
            //"ISteamWebAPIUtil/GetSupportedAPIList/v0001";
            //"ISteamUserStats/GetGlobalStatsForGame/v0001";
            //"ISteamUserStats/GetUserStatsForGame/v2";
            //"IClientStats_1046930/ReportEvent/v1";
            "ISteamApps/GetAppList/v1";



            var api_lists = process_data.ParseSupportedAPI(requested_interface, api_key, steam_id);


            foreach (var current_interface in api_lists.apilist.interfaces)
            {
                Console.WriteLine(current_interface.name);
            }


            //var game_info = process_data.ParseSupportedGameInfo(requested_interface, app_id, api_key, steam_id);

            //var method_info = process_data.ParseAPIMethod(requested_interface, api_key);

            //Console.WriteLine($"{method_info.name}, {method_info.description}");

            //Console.WriteLine($"{game_info.playerstats.gameName}: {game_info.playerstats.steamID}");


            //process_data.logon("devienlein", "Qytwer1100");

                
            


        }
    }
}
