using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Management;

namespace Wmi
{
    public class Util
    {
        public static void ShutdownHost(string hostName) {
            string adsiPath = string.Format(@"\\{0}\root\cimv2", hostName);
            System.Management.ManagementScope scope = new ManagementScope(adsiPath);
            // I've seen this, but I found not necessary:
            scope.Options.EnablePrivileges = true;
            ManagementPath osPath = new ManagementPath("Win32_OperatingSystem");
            ManagementClass os = new ManagementClass(scope, osPath, null);

            ManagementObjectCollection instances;
            try {
                instances = os.GetInstances();
            }
            catch (UnauthorizedAccessException exception) {
                throw new Exception("Not permitted to reboot the host: " + hostName, exception);
            }
            catch (System.Runtime.InteropServices.COMException exception) {
                if (exception.ErrorCode == -2147023174) {
                    throw new Exception("Could not reach the target host: " + hostName, exception);
                }
                throw; // Unhandled
            }
            foreach (ManagementObject instance in instances) {
                object result = instance.InvokeMethod("Shutdown", new object[] { });
                uint returnValue = (uint)result;

                if (returnValue != 0) {
                    throw new Exception("Failed to reboot host: " + hostName);
                }
            }
        }            
    }
}
