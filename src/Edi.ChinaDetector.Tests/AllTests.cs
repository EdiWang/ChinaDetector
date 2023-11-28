using NUnit.Framework;

namespace Edi.ChinaDetector.Tests;

[TestFixture]
public class AllTests
{
    [Test]
    public async Task DetectTimeZonePositive()
    {
        var httpClient = new HttpClient();
        var service = new ChinaDetectService(httpClient);

        var result = await service.Detect(DetectionMethod.Culture, new()
        {
            //TargetCulture = CultureInfo.GetCultureInfo("zh-CN"),
            //TargetUICulture = CultureInfo.GetCultureInfo("zh-CN"),
            TargetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time")
        });

        Assert.That(result.Rank, Is.EqualTo(1));
        Assert.That(result.PositiveMethod, Is.EqualTo(DetectionMethod.Culture));
    }

    [Test]
    public async Task DetectTimeZoneNegative()
    {
        var httpClient = new HttpClient();
        var service = new ChinaDetectService(httpClient);

        var result = await service.Detect(DetectionMethod.Culture, new()
        {
            TargetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
        });

        Assert.That(result.Rank, Is.EqualTo(0));
        Assert.That(result.PositiveMethod, Is.Null);
    }
}