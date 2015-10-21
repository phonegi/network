using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace IpScheme {
    public class Network {
        public IPAddress Address;
        public string Subnet;
        public Int64 NetworkValue;
        public Int64 LastHostValue;

        public Network(IPAddress address, string subnet) {
            byte[] bytes = address.GetAddressBytes();
            Address = address;
            Subnet = subnet;
            NetworkValue = (Int64)bytes[0] * 16777216 + bytes[1] * 65536 + bytes[2] * 256 + bytes[3];
            LastHostValue = NetworkValue + (Int64)(Math.Pow(2, (32 - Convert.ToInt32(subnet))) - 1);
        }

        public bool Overlap(Network network) {
            if (network.LastHostValue < NetworkValue) return false;
            if (network.NetworkValue < NetworkValue && NetworkValue < network.LastHostValue) return true;
            if (network.NetworkValue < NetworkValue && LastHostValue < network.LastHostValue) return true;
            if (NetworkValue < network.NetworkValue && network.LastHostValue < LastHostValue) return true;
            if (network.NetworkValue < LastHostValue && LastHostValue < network.LastHostValue) return true;
            if (LastHostValue < network.NetworkValue) return false;
            return true;
        }

        public static void SortNetworkList(ref List<Network> networks) {
            List<Network> newNetworks = new List<Network>(networks.Count);
            Network network;
            Int64 currentValue = Int64.MaxValue;
            int currentIndex = -1;

            while (networks.Count > 0) {
                currentValue = Int64.MaxValue;
                for (int index = 0; index < networks.Count; index++) {
                    network = networks[index];
                    if (network.NetworkValue < currentValue) {
                        currentIndex = index;
                        currentValue = network.NetworkValue;
                    }
                }
                newNetworks.Add(networks[currentIndex]);
                networks.RemoveAt(currentIndex);
            }
            networks = newNetworks;
        }

        public static List<Network> LoadFromFile(string filename) {
            List<Network> networks = new List<Network>();
            StreamReader reader;
            string pattern;
            string line;
            int lineNumber = 1;
            Match match;
            IPAddress ipAddress;
            string subnet;
            Network network;

            reader = File.OpenText(filename);
            //string pattern = @"(?:(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.)(?:(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.)(?:(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.)(?:(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))/(?:([1-2][0-9]|3[0-2]|[0-9]))";
            pattern = @"((?<ip>((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)))/(?<subnet>([1-2][0-9]|3[0-2]|[0-9])))";

            while (!reader.EndOfStream) {
                line = reader.ReadLine();
                if (line.Contains("subnet")) continue;
                match = Regex.Match(line, pattern);
                if (match.Success) {
                    ipAddress = IPAddress.Parse(match.Groups["ip"].Value);
                    subnet = match.Groups["subnet"].Value;
                    network = new Network(ipAddress, subnet);
                    try {
                        networks.Add(network);
                    }
                    catch {

#if DEBUG
                        Console.WriteLine("{0} is a duplicate. Line {1}", match.Groups["ip"].Value, lineNumber);
#endif

                    }
                }

#if DEBUG
                else {
                    Console.WriteLine("Skipping line {0}", lineNumber);
                }
#endif

                lineNumber++;
            }

#if DEBUG
            Console.WriteLine("Last Line {0}", lineNumber);
#endif

            reader.Close();
            return networks;
        }
    }
}
