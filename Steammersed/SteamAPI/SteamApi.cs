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
            public SteamApiGameStat playerstats { get; set; }
            //public List<SteamApiGameStat> playerstats { get; set; } <--- ONLY when "playerstats":[{...}]
        }
        //when we have "json":{...} its a class member called "json" and we must declare it as
        //class root { public SomeClassName json {get;set;} }
        //with contents in {...}
        //when we have "json":[] then its an array(so [{}] array of objects(classes)), makes sense, right?
        //in that case you should use List<SomeTypeName> json {get;set;}
        //when we have "json":"val" it's literraly a string, you can create class for it.
        //line above also true for boolean and number variables
        
        //Good luck!
        public class root
        { 
            public SteamApiAppList apilist { get; set; }
        }

    }
}
