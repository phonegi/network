using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using IpScheme;

namespace CompareSchemes {
    class Program {
        static void Main(string[] args) {
            if (args.Length < 2) return;
            if (!File.Exists(args[0])) return;
            if (!File.Exists(args[1])) return;

            List<Network> aNetworks = Network.LoadFromFile(args[0]);
            List<Network> bNetworks = Network.LoadFromFile(args[1]);
            Network.SortNetworkList(ref aNetworks);
            Network.SortNetworkList(ref bNetworks);

            foreach (Network aNetwork in aNetworks) {
                foreach (Network bNetwork in bNetworks) {
                    if (aNetwork.Overlap(bNetwork)) {
                        Console.WriteLine("{0}/{1} overlaps {2}/{3}", aNetwork.Address, aNetwork.Subnet, bNetwork.Address, bNetwork.Subnet);
                    }
                }
            }

        }
    }
}
