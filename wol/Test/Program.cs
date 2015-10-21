using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;

namespace Network {
    class Program {
        static void Main(string[] args) {
            if (args.Length < 1) return;
            WakeOnLan.WakeUp(args[0]);
        }
    }
}
