
using System;
using System.Net;
using System.Net.Sockets;


namespace WindowsLocalNetwork.Helpers
{
    internal class GetIP
    {

        public static IPAddress MyIP;
        public static string MyHOST;

        static GetIP()
        {
           
                IPAddress[] localIp = Dns.GetHostAddresses(Dns.GetHostName());

                foreach (IPAddress address in localIp)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        MyIP = address;
                        break;
                    }
                }

            MyHOST = Dns.GetHostEntry(MyIP).HostName.Split('.')[0];
        }

    }
}
