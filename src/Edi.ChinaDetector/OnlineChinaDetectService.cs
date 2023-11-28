﻿using System.ComponentModel;

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

    private Task<(int Rank, string IPAddress)> DetectByIPAddress(HttpClient httpClient) => new IPChinaDetector(httpClient).Detect();

    private static Task<int> DetectByGFWTest() => GFWChinaDetector.Detect();
}

public class GeoIPResult
{
    public string CountryCode { get; set; }

    public string Query { get; set; }
}