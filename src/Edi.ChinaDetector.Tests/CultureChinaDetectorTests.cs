using NUnit.Framework;
using System.Globalization;

namespace Edi.ChinaDetector.Tests;

[TestFixture]
public class CultureChinaDetectorTests
{
    [Test]
    public void DetectPositiveCulture()
    {
        var cul = CultureInfo.GetCultureInfo("zh-CN");
        var uiCul = CultureInfo.GetCultureInfo("en-US");

        var detector = new CultureChinaDetector(cul, uiCul);

        var result = detector.Detect();

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void DetectPositiveBoth()
    {
        var cul = CultureInfo.GetCultureInfo("zh-CN");
        var uiCul = CultureInfo.GetCultureInfo("zh-CN");

        var detector = new CultureChinaDetector(cul, uiCul);

        var result = detector.Detect();

        Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public void DetectPositiveUICulture()
    {
        var cul = CultureInfo.GetCultureInfo("en-US");
        var uiCul = CultureInfo.GetCultureInfo("zh-CN");

        var detector = new CultureChinaDetector(cul, uiCul);

        var result = detector.Detect();

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void DetectNegative()
    {
        var cul = CultureInfo.GetCultureInfo("en-US");
        var uiCul = CultureInfo.GetCultureInfo("en-US");

        var detector = new CultureChinaDetector(cul, uiCul);

        var result = detector.Detect();

        Assert.That(result, Is.EqualTo(0));
    }
}