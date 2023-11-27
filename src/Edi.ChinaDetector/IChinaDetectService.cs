namespace Edi.ChinaDetector;

public interface IChinaDetectService
{
    public Task<ChinaDetectResult> Detect(DetectionMethod method);
}

[Flags]
public enum DetectionMethod : int
{
    TimeZone,
    Culture,
    IPAddress,
    GFWTest
}

public class ChinaDetectResult
{
    public int Rank { get; set; }
    public DetectionMethod PositiveMethod { get; set; }
    public string IPAddress { get; set; }
}