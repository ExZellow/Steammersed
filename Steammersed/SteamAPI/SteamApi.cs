using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steammersed
{
    public static class SteamApi
    {
       public class SteamMethodParameter
        {
            public string name { get; set; }
            public string type { get; set; }
            public bool optional { get; set; }
            public string description { get; set; }
        }
        public class SteamApiMethod
        {
            public string name { get; set; }
            public int version { get; set; }
            public string httpmethod { get; set; }
            public List<SteamMethodParameter> parameters { get; set; }
            public string description { get; set; }
        }
        public class SteamApiInterface
        {
            public string name { get; set; }
            public List<SteamApiMethod> methods { get; set; }
        }
        
        public class SteamApiList
        {
            public List<SteamApiInterface> interfaces { get; set; }
        }

        public class SteamApiGameStat
        {
            public string steamID { get; set; }
            public string gameName { get; set; }
        }

        public class SteamApiGameInfo
        {
            public SteamApiGameStat playerstats { get; set; }    
        }
        
        public class RootSteamApiList
        { 
            public SteamApiList apilist { get; set; }
        }

        public class SteamApp
        {
            public string appid { get; set; }
            public string name { get; set; }
        }

        public class SteamAppList
        {
            public List<SteamApp> apps { get; set; }
        }

        public class RootSteamApp
        {
            public SteamAppList applist { get; set; }
        }

    }
}
