using System.Globalization;

namespace Edi.ChinaDetector;

public interface IChinaDetectService
{
    public Task<ChinaDetectResult> Detect(DetectionMethod method, RegionInfo regionInfo = null);
}

[Flags]
public enum DetectionMethod : int
{
    TimeZone = 1,
    Culture = 2,
    Behavior = 4,
    IPAddress = 8,
    GFWTest = 16,
    NetworkGateway = 32,

    AllOffline = TimeZone | Culture | Behavior,
    AllOnline = IPAddress | GFWTest | NetworkGateway
}

public class ChinaDetectResult
{
    public int Rank { get; set; }
    public List<DetectionMethod> PositiveMethods { get; set; }
    public string IPAddress { get; set; }
}

public class RegionInfo
{
    public CultureInfo TargetCulture { get; set; } = CultureInfo.CurrentCulture;

    public CultureInfo TargetUICulture { get; set; } = CultureInfo.CurrentUICulture;

    public TimeZoneInfo TargetTimeZone { get; set; } = TimeZoneInfo.Local;

    public RegionInfo CurrentRegion => new();
}
