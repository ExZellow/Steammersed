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

            var requested_interface = "IPlayerService/GetOwnedGames/v0001/?access_token=";
            //"ISteamWebAPIUtil/GetSupportedAPIList/v0001/?access_token=";
            var api_key = "E3E5FDDA82E49614AA1D9F65F6C2A8E2";
            var steamid = "76561198329187801";


            var k = x.ParseSupportedAPI(requested_interface, api_key, steamid);
            foreach (var current_interface in k.apilist.interfaces)
            {
                Console.WriteLine(current_interface.name);
            }


        }
    }
}
