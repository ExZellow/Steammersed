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

            var k = x.ParseSupportedAPI();
            foreach (var e in k.apilist.interfaces)
            {
                Console.WriteLine(e.name);
            }
        }
    }
}
