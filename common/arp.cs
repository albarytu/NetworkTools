using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPcap;
using SharpPcap.LibPcap;
using System.Diagnostics;

namespace NetworkTools.Common
{
    public class arp
    {
        public static string formatMac(System.Net.NetworkInformation.PhysicalAddress mac)
        {
            string macstring = mac.ToString();
            return string.Join(":", Enumerable.Range(0, macstring.Length / 2).Select(i => macstring.Substring(i * 2, 2)));
        }
        public class arpResult
        {
            public System.Net.IPAddress dest;
            public System.Net.NetworkInformation.PhysicalAddress mac;
            public long elapsedMs;
            public override string ToString()
            {
                if (mac==null)
                {
                    return string.Format("Timeout, no mac address found for IP {0}", dest);
                } else
                {
                    return string.Format("IP {0} at {1} ({2} ms)", dest, formatMac(mac), elapsedMs);
                }
            }
        }
        public static arpResult Resolve(System.Net.IPAddress dest, LibPcapLiveDevice dev=null)
        {
            if (dev==null)
            {
                dev = device.directLinkDevice(dest);
            }
            ARP arper = new ARP(dev);
            Stopwatch sw = Stopwatch.StartNew();
            System.Net.NetworkInformation.PhysicalAddress mac = arper.Resolve(dest);
            sw.Stop();
            long ms = sw.ElapsedMilliseconds;
            return new arpResult { dest=dest, mac = mac, elapsedMs = ms };
        }
    }
}
