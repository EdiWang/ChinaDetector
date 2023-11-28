using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;

namespace Edi.ChinaDetector;

public class ChinaDetectService(HttpClient httpClient) : IChinaDetectService
{
    public async Task<ChinaDetectResult> Detect(DetectionMethod method, RegionInfo regionInfo = null)
    {
        var result = new ChinaDetectResult();

        if (method.HasFlag(DetectionMethod.TimeZone))
        {
            var r1 = DetectByTimeZone(regionInfo?.TargetTimeZone);
            result.Rank += r1;

            if (r1 > 0) result.PositiveMethod |= DetectionMethod.Culture;
        }

        if (method.HasFlag(DetectionMethod.Culture))
        {
            var r2 = DetectByCulture(regionInfo?.TargetCulture, regionInfo?.TargetUICulture);
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

    private static int DetectByTimeZone(TimeZoneInfo timeZone)
    {
        timeZone ??= TimeZoneInfo.Local;

        if (timeZone.Id == "China Standard Time" ||
            timeZone.StandardName.Contains("china", StringComparison.CurrentCultureIgnoreCase))
        {
            return 1;
        }

        return 0;
    }

    private static int DetectByCulture(CultureInfo culture = null, CultureInfo uiCulture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        uiCulture ??= CultureInfo.CurrentUICulture;

        int rank = 0;

        if (culture.Name == "zh-CN" ||
            culture.Name == "zh-Hans" ||
            culture.Name == "zh-Hans-CN" ||
            culture.EnglishName.Contains("china", StringComparison.InvariantCultureIgnoreCase))
        {
            rank++;
        }

        if (uiCulture.Name == "zh-CN" ||
            uiCulture.Name == "zh-Hans" ||
            uiCulture.Name == "zh-Hans-CN" ||
            uiCulture.EnglishName.Contains("china", StringComparison.InvariantCultureIgnoreCase))
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