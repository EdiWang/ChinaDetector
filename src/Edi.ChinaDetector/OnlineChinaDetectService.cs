using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;

namespace Edi.ChinaDetector;

public class OnlineChinaDetectService(HttpClient httpClient) : IChinaDetectService
{
    public async Task<ChinaDetectResult> Detect(DetectionMethod method, RegionInfo regionInfo = null)
    {
        var result = new ChinaDetectResult { PositiveMethods = new() };

        if (method.HasFlag(DetectionMethod.TimeZone))
        {
            throw new InvalidEnumArgumentException("Please use OfflineChinaDetectService");
        }

        if (method.HasFlag(DetectionMethod.Culture))
        {
            throw new InvalidEnumArgumentException("Please use OfflineChinaDetectService");
        }

        if (method.HasFlag(DetectionMethod.IPAddress))
        {
            var r3 = await DetectByIPAddress(httpClient);
            result.Rank += r3.Rank;

            if (r3.Rank > 0)
            {
                result.PositiveMethods.Add(DetectionMethod.IPAddress);
                result.IPAddress = r3.IPAddress;
            }
        }

        if (method.HasFlag(DetectionMethod.GFWTest))
        {
            var r4 = await DetectByGFWTest();
            result.Rank += r4;

            if (r4 > 0) result.PositiveMethods.Add(DetectionMethod.GFWTest);
        }

        return result;
    }

    private async Task<(int Rank, string IPAddress)> DetectByIPAddress(HttpClient httpClient)
    {
        int rank = 0;
        string ip = null;

        try
        {
            const string geoIpServiceUrl = "http://ip-api.com/json/";
            var response = await httpClient.GetFromJsonAsync<GeoIPResult>(geoIpServiceUrl);

            ip = response.Query;
            if (response.CountryCode == "CN") rank++;
        }
        catch (HttpRequestException)
        {
            rank++;
        }
        catch (Exception)
        {
            rank++;
        }

        return (rank, ip);
    }

    private static async Task<int> DetectByGFWTest()
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

public class GeoIPResult
{
    public string CountryCode { get; set; }

    public string Query { get; set; }
}