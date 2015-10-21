using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Network
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress[] ip_addresses;
            IPEndPoint ip_end_point;
            UdpClient udp_client;
            byte[] mac_bytes;

            test();

            //verify arg[0] is valid MAC address
            if (!validate_args(args))
            {
                show_help();
                return;
            }

            mac_bytes = convert_mac_address(args[0]);

            ip_addresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip_address in ip_addresses)
            {
                if (IPAddress.IsLoopback(ip_address) || ip_address.AddressFamily != AddressFamily.InterNetwork) continue;
                ip_end_point = new IPEndPoint(ip_address, 0);
                udp_client = new UdpClient(ip_end_point);
                udp_client.Connect(IPAddress.Broadcast, 80);
                byte[] data = magic_packet_bytes(mac_bytes);
                udp_client.Send(data, data.Length);
            }

        }

        static void test() {
            PhysicalAddress mac = PhysicalAddress.Parse("a3-22-d3-12-0e-6b".ToUpper());
            string s = mac.ToString();
        }

        private static bool validate_args(string[] args)
        {
            //Must only be one command line argument
            if (args.Length != 1) return false;

            List<char> mac_chars = new List<char>();
            char[] hex_chars = { 'a', 'b', 'c', 'd', 'e', 'f' };

            //Loop through each character in the command line argument.
            //Save only hex letters and digits. Convert letters to lower case.
            foreach (char c in args[0].ToCharArray()) {
                if (Char.IsDigit(c)) {
                    mac_chars.Add(c);
                }
                else if (Char.IsLetter(c)) {
                    if (!hex_chars.Contains(Char.ToLower(c))) {
                        return false;
                    }

                    mac_chars.Add(Char.ToLower(c));
                }
            }

            //Verify that 12 characters were provided
            if (mac_chars.Count != 12) {
                return false;
            }

            //Convert the argument to a string of lowercase letters and digits
            args[0] = new String(mac_chars.ToArray());
            return true;
        }

        private static byte[] convert_mac_address(string mac_string) {
            byte[] mac_bytes = new byte[6];

            //convert each set of 2 chars to a single byte
            for (int ndx = 0; ndx < 6; ndx++) {
                mac_bytes[ndx] = Convert.ToByte(mac_string.Substring(ndx * 2, 2), 16);
            }

            return mac_bytes;
        }

        private static byte[] magic_packet_bytes(byte[] mac_bytes)
        {
            byte[] data = new byte[102];
            int ndx = 0;

            //first 6 bytes of magic packet are broadcast address "FF-FF-FF-FF-FF-FF"
            for (ndx = 0; ndx < 6; ndx++) {
                data[ndx] = 0xFF;
            }
            
            //next bytes are 16 copies of the MAC address
            for (int ndx_base=6; ndx_base<102; ndx_base+=6)
            {
                for (ndx=0; ndx<6; ndx++)
                {
                    data[ndx_base + ndx] = mac_bytes[ndx];
                }
            }
            return data;
        }

        private static void show_help()
        {
            Console.Out.WriteLine("Sends a Wake-On-LAN Magic Packet to a device.\n");
            Console.Out.WriteLine("WOL mac_address\n");
            Console.Out.WriteLine("   mac_address   The 6-byte hex address separated by dashes. Ex: 3e-2c-ac-7f-ff-1d");
        }
    }
}
