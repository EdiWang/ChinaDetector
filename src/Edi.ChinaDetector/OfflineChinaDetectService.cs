using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Edi.ChinaDetector;

public class OfflineChinaDetectService : IChinaDetectService
{
    public async Task<ChinaDetectResult> Detect(DetectionMethod method, RegionInfo regionInfo = null)
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
            var r2 = await DetectByCulture(regionInfo?.TargetCulture, regionInfo?.TargetUICulture);
            result.Rank += r2;

            if (r2 > 0) result.PositiveMethods.Add(DetectionMethod.Culture);
        }

        if (method.HasFlag(DetectionMethod.Behavior))
        {
            var r3 = DetectByBehavior();
            result.Rank += r3;

            var r4 = await DetectByNpm();
            result.Rank += r4;

            if (r3 > 0 || r4 > 0) result.PositiveMethods.Add(DetectionMethod.Behavior);
        }

        if (method.HasFlag(DetectionMethod.IPAddress) || method.HasFlag(DetectionMethod.GFWTest))
        {
            throw new InvalidEnumArgumentException("Please use OnlineChinaDetectService");
        }

        return result;
    }

    private static int DetectByTimeZone(TimeZoneInfo timeZone) => new TimeZoneChinaDetector(timeZone).Detect();

    private static async Task<int> DetectByCulture(CultureInfo culture = null, CultureInfo uiCulture = null) => await new CultureChinaDetector(culture, uiCulture).Detect();

    private static int DetectByBehavior() => new BehaviorChinaDetector().Detect();

    private static async Task<int> DetectByNpm() => await new NpmChinaDetector().Detect();
}