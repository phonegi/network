using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Management;
using System.Net;

namespace Wmi
{
    public class Util
    {
        public enum ShutdownResult {
            SUCCESS,
            ERROR_AUTHORIZATION,
            ERROR_UNREACHABLE,
            ERROR_REBOOT_FAILURE,
            ERROR_UNKNOWN
        }

        public static ShutdownResult ShutdownHost(string hostName) {
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
            catch (UnauthorizedAccessException) {
                //throw new Exception("Not permitted to reboot the host: " + hostName, exception);
                return ShutdownResult.ERROR_AUTHORIZATION;
            }
            catch (System.Runtime.InteropServices.COMException exception) {
                if (exception.ErrorCode == -2147023174) {
                    //throw new Exception("Could not reach the target host: " + hostName, exception);
                    return ShutdownResult.ERROR_UNREACHABLE;
                }
                //throw; // Unhandled
                return ShutdownResult.ERROR_UNKNOWN;
            }
            foreach (ManagementObject instance in instances) {
                object result = instance.InvokeMethod("Shutdown", new object[] { });
                uint returnValue = (uint)result;

                if (returnValue != 0) {
                    //throw new Exception("Failed to reboot host: " + hostName);
                    return ShutdownResult.ERROR_REBOOT_FAILURE;
                }
            }
            return ShutdownResult.SUCCESS;
        }     
        
        public static ShutdownResult ShutdownHost(IPAddress ip) {
            return ShutdownHost(ip.ToString());
        }
    }
}
