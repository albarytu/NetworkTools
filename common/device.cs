using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPcap;
using SharpPcap.LibPcap;
using System.Net;

namespace NetworkTools.Common
{
    public class device
    {
        public static IEnumerable<string> getDeviceNames()
        {
            foreach (LibPcapLiveDevice device in LibPcapLiveDeviceList.Instance)
            {
                yield return device.Interface.FriendlyName;
            }
        }
        public static LibPcapLiveDevice deviceByName(string name)
        {
            foreach (LibPcapLiveDevice device in LibPcapLiveDeviceList.Instance)
            {
                if (name == device.Interface.FriendlyName)
                {
                    return device;
                }
            }
            return null;
        }
        public static LibPcapLiveDevice directLinkDevice(IPAddress dest)
        {
            foreach (LibPcapLiveDevice device in LibPcapLiveDeviceList.Instance)
            {
                device.Open();
                try
                {
                    foreach (PcapAddress address in device.Addresses)
                    {
                        if (address.Addr.type==Sockaddr.AddressTypes.AF_INET_AF_INET6)
                        {
                            IPAddress devaddr = address.Addr.ipAddress;
                            if (devaddr.AddressFamily == dest.AddressFamily)
                            {
                                ipnetwork net = new ipnetwork(devaddr, address.Netmask.ipAddress);
                                if (net.matches(dest))
                                {
                                    return device;
                                }
                            }
                        }
                    }
                } finally
                {
                    device.Close();
                }
            }
            return null;
        }
        
        public static IEnumerable<IPAddress> directLinkAddresses(LibPcapLiveDevice device)
        {
            device.Open();
            try
            {
                foreach (PcapAddress address in device.Addresses)
                {
                    if (address.Addr.type == Sockaddr.AddressTypes.AF_INET_AF_INET6)
                    {
                        ipnetwork net = new ipnetwork(address.Addr.ipAddress, address.Netmask.ipAddress);
                        foreach (IPAddress a in net.usable())
                        {
                            yield return a;
                        }
                    }
                }
            }
            finally
            {
                device.Close();
            }
        }
        
    }
}
