using NUnit.Framework;
using System.Globalization;

namespace Edi.ChinaDetector.Tests;

[TestFixture]
public class CultureChinaDetectorTests
{
    [Test]
    public async Task DetectPositiveCulture()
    {
        var cul = CultureInfo.GetCultureInfo("zh-CN");
        var uiCul = CultureInfo.GetCultureInfo("en-US");

        var detector = new CultureChinaDetector(cul, uiCul);

        var result = await detector.Detect();

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task DetectPositiveBoth()
    {
        var cul = CultureInfo.GetCultureInfo("zh-CN");
        var uiCul = CultureInfo.GetCultureInfo("zh-CN");

        var detector = new CultureChinaDetector(cul, uiCul);

        var result = await detector.Detect();

        Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public async Task DetectPositiveUICulture()
    {
        var cul = CultureInfo.GetCultureInfo("en-US");
        var uiCul = CultureInfo.GetCultureInfo("zh-CN");

        var detector = new CultureChinaDetector(cul, uiCul);

        var result = await detector.Detect();

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task DetectNegative()
    {
        var cul = CultureInfo.GetCultureInfo("en-US");
        var uiCul = CultureInfo.GetCultureInfo("en-US");

        var detector = new CultureChinaDetector(cul, uiCul);

        var result = await detector.Detect();

        Assert.That(result, Is.EqualTo(0));
    }
}