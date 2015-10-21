using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

namespace IpScheme {

    class Program {

        static void Main(string[] args) {
            Network network;
            List<Network> networks;

            if (args.Length < 1) return;
            if (!File.Exists(args[0])) return;

            networks = Network.LoadFromFile(args[0]);

            Network.SortNetworkList(ref networks);
            for (int index = 0; index < networks.Count; index++) {
                network = networks[index];
                Console.WriteLine("{0}/{1}", network.Address.ToString(), network.Subnet);
            }
        }
    }
}
