using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BinaryAnalysis.Helpers.Network
{
    public static class NetworkCommons
    {
        public static IPAddress[] GetAllUnicastAddresses()
        {
            // This works on both Mono and .NET , but there is a difference: it also
            // includes the LocalLoopBack so we need to filter that one out
            List<IPAddress> Addresses = new List<IPAddress>();
            // Obtain a reference to all network interfaces in the machine
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                foreach (IPAddressInformation uniCast in properties.UnicastAddresses)
                {
                    // Ignore loop-back addresses & IPv6
                    if (!IPAddress.IsLoopback(uniCast.Address) && uniCast.Address.AddressFamily != AddressFamily.InterNetworkV6)
                        Addresses.Add(uniCast.Address);
                }

            }
            return Addresses.ToArray();
        }
    }
}
