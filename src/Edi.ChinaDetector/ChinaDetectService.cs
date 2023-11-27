using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;

namespace Edi.ChinaDetector;

public class ChinaDetectService(HttpClient httpClient) : IChinaDetectService
{
    public async Task<ChinaDetectResult> Detect(DetectionMethod method)
    {
        var result = new ChinaDetectResult();

        if (method.HasFlag(DetectionMethod.TimeZone))
        {
            var r1 = DetectByTimeZone();
            result.Rank += r1;

            if (r1 > 0) result.PositiveMethod |= DetectionMethod.Culture;
        }

        if (method.HasFlag(DetectionMethod.Culture))
        {
            var r2 = DetectByCulture();
            result.Rank += r2;

            if (r2 > 0) result.PositiveMethod |= DetectionMethod.Culture;
        }

        if (method.HasFlag(DetectionMethod.IPAddress))
        {
            var r3 = await DetectByIPAddress(httpClient);
            result.Rank += r3.Rank;

            if (r3.Rank > 0)
            {
                result.PositiveMethod |= DetectionMethod.IPAddress;
                result.IPAddress = r3.IPAddress;
            }
        }

        if (method.HasFlag(DetectionMethod.GFWTest))
        {
            var r4 = await DetectByGFWTest();
            result.Rank += r4;

            if (r4 > 0) result.PositiveMethod |= DetectionMethod.GFWTest;
        }

        return result;
    }

    private static int DetectByTimeZone()
    {
        if (TimeZoneInfo.Local.Id == "China Standard Time" ||
            TimeZoneInfo.Local.StandardName.Contains("china", StringComparison.CurrentCultureIgnoreCase))
        {
            return 1;
        }

        return 0;
    }

    private static int DetectByCulture()
    {
        int rank = 0;

        if (CultureInfo.CurrentCulture.Name == "zh-CN" ||
            CultureInfo.CurrentCulture.Name == "zh-Hans" ||
            CultureInfo.CurrentCulture.Name == "zh-Hans-CN" ||
            CultureInfo.CurrentCulture.EnglishName.ToLowerInvariant().Contains("china"))
        {
            rank++;
        }

        if (CultureInfo.CurrentUICulture.Name == "zh-CN" ||
            CultureInfo.CurrentUICulture.Name == "zh-Hans" ||
            CultureInfo.CurrentUICulture.Name == "zh-Hans-CN" ||
            CultureInfo.CurrentUICulture.EnglishName.ToLowerInvariant().Contains("china"))
        {
            rank++;
        }

        return rank;
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