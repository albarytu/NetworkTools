using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Net;
using System.Net.Sockets;

namespace NetworkTools.Common
{
    public class ipnetwork
    {
        private AddressFamily family;
        private byte[] networkBytes;
        private BigInteger networkInt;
        private byte[] maskBytes;
        private byte[] reverseMaskBytes;
        private BigInteger reverseMaskInt;
        private byte[] broadcastBytes;
        

        public ipnetwork(IPAddress address, IPAddress mask)
        {
            this.family = address.AddressFamily;
            byte[] ipBytes = address.GetAddressBytes();
            this.maskBytes = mask.GetAddressBytes();
            this.networkBytes = new byte[ipBytes.Length];
            this.reverseMaskBytes = new byte[ipBytes.Length];
            this.broadcastBytes= new byte[ipBytes.Length];
            for (int i = 0; i < ipBytes.Length; i++)
            {
                networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
                reverseMaskBytes[i] = (byte)~maskBytes[i];
                broadcastBytes[i] = (byte)(networkBytes[i] | reverseMaskBytes[i]);
            }

            byte[] aux= new byte[ipBytes.Length];
            Array.Copy(networkBytes, aux, ipBytes.Length);
            Array.Reverse(aux);
            networkInt = new BigInteger(aux);

            Array.Copy(reverseMaskBytes, aux, ipBytes.Length);
            Array.Reverse(aux);
            reverseMaskInt = new BigInteger(aux);

        }
        public bool matches(IPAddress address)
        {
            byte[] ipBytes = address.GetAddressBytes();
            for (int i = 0; i < ipBytes.Length; i++)
            {
                byte byteN = (byte)(ipBytes[i] & maskBytes[i]);
                if (byteN != networkBytes[i])
                {
                    return false;
                }
            }
            return true;
        }
        public IPAddress network
        {
            get
            {
                return new IPAddress(networkBytes);
            }
        }
        public IPAddress broadcast
        {
            get
            {
                return new IPAddress(broadcastBytes);
            }
        }

        public static IPAddress toIPAddress(BigInteger intvalue, int length)
        {
            byte[] ipBytes = new byte[length];
            byte[] valBytes = intvalue.ToByteArray();
            for (int i=0; i<ipBytes.Length; i++)
            {
                if (valBytes.Length > i)
                {
                    ipBytes[i] = valBytes[i];
                } else
                {
                    ipBytes[i] = 0;
                }
            }
            Array.Reverse(ipBytes);
            return new IPAddress(ipBytes);
        }

        private BigInteger first
        {
            get
            {
                BigInteger first;
                if (reverseMaskInt != 0)
                {
                    first = networkInt + 1;
                } else
                {
                    first = networkInt;
                }
                return first;
            }
        }

        private BigInteger last
        {
            get
            {
                BigInteger last;
                if (reverseMaskInt != 0)
                {
                    last = networkInt + reverseMaskInt - 1;
                }
                else
                {
                    last = networkInt;
                }
                return last;
            }
        }

        public IPAddress firstIP
        {
            get
            {
                return toIPAddress(this.first, this.networkBytes.Length);
            }
        }

        public IPAddress lastIP
        {
            get
            {
                return toIPAddress(this.last, this.networkBytes.Length);
            }
        }

        public IEnumerable<IPAddress> usable()
        {
            if (reverseMaskInt == 0)
            {
                yield return new IPAddress(this.networkBytes);
            } else
            {
                BigInteger f = this.first;
                BigInteger l = this.last;
                if (f > l)
                {
                    yield return toIPAddress(l, this.networkBytes.Length);
                    yield return toIPAddress(f, this.networkBytes.Length);
                }
                else {
                    for (BigInteger x=f; x<=l; x++)
                    {
                        yield return toIPAddress(x, this.networkBytes.Length);
                    }
                }
            }
        }

    }
}
