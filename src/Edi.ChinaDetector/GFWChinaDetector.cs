using System.Net;
using System.Net.Sockets;

namespace Edi.ChinaDetector;

public class GFWChinaDetector
{
    public static async Task<int> Detect()
    {
        int rank = 0;

        try
        {
            var ip = (await Dns.GetHostAddressesAsync("www.google.com"))[0];
            if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString().StartsWith("172.217."))
            {
                rank++;
            }
        }
        catch (SocketException)
        {
            rank++;
        }
        catch (Exception)
        {
            rank++;
        }

        return rank;
    }
}