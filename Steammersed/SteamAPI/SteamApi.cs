using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steammersed
{
    /*
     * following json format for GetSupportedAPIList
    "applist" : 
    {
        "interfaces": [
          {
            "name": "IClientStats_1046930,
            "methods": [
             {
                "name": "ReportEvent",
                "version":1,
                "httpmethod":"POST",
                "parameters":[]
             } 
            ]
          },
          {interface},
        ]
    }
    */
    public static class SteamApi
    {
       public class SteamMethodParam
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
            public List<SteamMethodParam> parameters { get; set; }
        }
        public class SteamApiInterface
        {
            public string name { get; set; }
            public List<SteamApiMethod> methods { get; set; }
        }
        
        public class SteamApiAppList
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
            public List<SteamApiGameStat> playerstats { get; set; }
        }

        public class root
        { 
            public SteamApiAppList apilist { get; set; }
        }

    }
}
