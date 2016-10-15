using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkTools.Common;

namespace arping
{
    class Program
    {
        static void Usage()
        {
            Console.WriteLine("Required: ip address");
        }
        static void Main(string[] args)
        {
            if ((args == null) || (args.Length < 1))
            {
                Usage();
                return;
            }
            bool keepGoing = true;
            Console.CancelKeyPress += delegate {
                keepGoing = false;

            };
            string ip = args[0];
            System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse(ip);
            while (keepGoing)
            {
                arp.arpResult result = arp.Resolve(ipaddress);
                Console.WriteLine("{0}", result.ToString());
                for (int i=0; i<5; i++)
                {
                    if (keepGoing) Thread.Sleep(200);
                }                
            }
        }
    }
}
