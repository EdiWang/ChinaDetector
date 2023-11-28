using System.ComponentModel;
using System.Globalization;

namespace Edi.ChinaDetector;

public class OfflineChinaDetectService : IChinaDetectService
{
    public Task<ChinaDetectResult> Detect(DetectionMethod method, RegionInfo regionInfo = null)
    {
        var result = new ChinaDetectResult { PositiveMethods = new() };

        if (method.HasFlag(DetectionMethod.TimeZone))
        {
            var r1 = DetectByTimeZone(regionInfo?.TargetTimeZone);
            result.Rank += r1;

            if (r1 > 0) result.PositiveMethods.Add(DetectionMethod.TimeZone);
        }

        if (method.HasFlag(DetectionMethod.Culture))
        {
            var r2 = DetectByCulture(regionInfo?.TargetCulture, regionInfo?.TargetUICulture);
            result.Rank += r2;

            if (r2 > 0) result.PositiveMethods.Add(DetectionMethod.Culture);
        }

        if (method.HasFlag(DetectionMethod.IPAddress))
        {
            throw new InvalidEnumArgumentException("Please use OnlineChinaDetectService");
        }

        if (method.HasFlag(DetectionMethod.GFWTest))
        {
            throw new InvalidEnumArgumentException("Please use OnlineChinaDetectService");
        }

        return Task.FromResult(result);
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
}

public class GeoIPResult
{
    public string CountryCode { get; set; }

    public string Query { get; set; }
}