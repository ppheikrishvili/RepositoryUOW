using System.Net;
using System.Net.Sockets;

namespace RepositoryUOWDomain.Shared.Common
{
    public class IpHelper
    {
        public static string GetIpAddress() =>
            $"{Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(w => 
                w.AddressFamily == AddressFamily.InterNetwork)}";
    }
}
