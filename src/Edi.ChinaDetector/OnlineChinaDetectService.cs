using System.ComponentModel;

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
            var r3 = await DetectByIPAddress();
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

        if (method.HasFlag(DetectionMethod.NetworkGateway))
        {
            var r5 = DetectByNetworkGateway();
            result.Rank += r5;

            if (r5 > 0) result.PositiveMethods.Add(DetectionMethod.NetworkGateway);
        }

        return result;
    }

    private Task<(int Rank, string IPAddress)> DetectByIPAddress() => new IPChinaDetector(httpClient).Detect();

    private Task<int> DetectByGFWTest() => new GFWChinaDetector(httpClient).Detect();

    private int DetectByNetworkGateway() => new NetworkGatewayChinaDetector().Detect("127.0.0.1");
}

public class GeoIPResult
{
    public string CountryCode { get; set; }

    public string Query { get; set; }
}