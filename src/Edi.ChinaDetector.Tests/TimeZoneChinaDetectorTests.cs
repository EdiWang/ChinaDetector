using NUnit.Framework;

namespace Edi.ChinaDetector.Tests;

[TestFixture]
public class TimeZoneChinaDetectorTests
{
    [Test]
    public void DetectPositive()
    {
        var detector = new TimeZoneChinaDetector(TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));

        var result = detector.Detect();

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void DetectNegative()
    {
        var detector = new TimeZoneChinaDetector(TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

        var result = detector.Detect();

        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void DetectTaiwanPositive()
    {
        var detector = new TimeZoneChinaDetector(TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time"), true);
        var result = detector.Detect();
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void DetectTaiwanNegative()
    {
        var detector = new TimeZoneChinaDetector(TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time"));
        var result = detector.Detect();
        Assert.That(result, Is.EqualTo(0));
    }
}