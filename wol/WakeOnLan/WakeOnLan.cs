using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Network
{
    public class WakeOnLan
    {
        public static void WakeUp(string macString) {
            PhysicalAddress macAddress;
            try {
                macAddress = PhysicalAddress.Parse(macString);
            }
            catch {
                return;
            }

            WakeUp(macAddress);
        }

        public static void WakeUp(PhysicalAddress macAddress) {
            IPAddress[] ipAddresses;
            IPEndPoint ipEndPoint;
            UdpClient udpClient;

            ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ipAddress in ipAddresses) {
                if (IPAddress.IsLoopback(ipAddress) || ipAddress.AddressFamily != AddressFamily.InterNetwork) continue;
                ipEndPoint = new IPEndPoint(ipAddress, 0);
                udpClient = new UdpClient(ipEndPoint);
                udpClient.Connect(IPAddress.Broadcast, 80);
                byte[] data = GetMagicPacketBytes(macAddress);
                udpClient.Send(data, data.Length);
            }
        }

        private static byte[] GetMagicPacketBytes(PhysicalAddress macAddress) {
            byte[] data = new byte[102];
            byte[] macBytes = macAddress.GetAddressBytes();
            int ndx = 0;

            //first 6 bytes of magic packet are broadcast address "FF-FF-FF-FF-FF-FF"
            for (ndx = 0; ndx < 6; ndx++) {
                data[ndx] = 0xFF;
            }

            //next bytes are 16 copies of the MAC address
            for (int ndxBase = 6; ndxBase < 102; ndxBase += 6) {
                for (ndx = 0; ndx < 6; ndx++) {
                    data[ndxBase + ndx] = macBytes[ndx];
                }
            }
            return data;
        }

    }
}
