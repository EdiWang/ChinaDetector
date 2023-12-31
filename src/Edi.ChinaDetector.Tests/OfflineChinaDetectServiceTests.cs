﻿using NUnit.Framework;
using System.Globalization;

namespace Edi.ChinaDetector.Tests;

[TestFixture]
public class OfflineChinaDetectServiceTests
{
    [Test]
    public async Task DetectTimeZonePositive()
    {
        var service = new OfflineChinaDetectService();

        var result = await service.Detect(DetectionMethod.TimeZone, new()
        {
            TargetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time")
        });

        Assert.That(result.Rank, Is.EqualTo(1));
        Assert.That(result.PositiveMethods, Is.Not.Null);
        Assert.That(result.PositiveMethods.Count == 1, Is.True);
        Assert.That(result.PositiveMethods.First(), Is.EqualTo(DetectionMethod.TimeZone));
    }

    [Test]
    public async Task DetectTimeZoneNegative()
    {
        var service = new OfflineChinaDetectService();

        var result = await service.Detect(DetectionMethod.TimeZone, new()
        {
            TargetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
        });

        Assert.That(result.Rank, Is.EqualTo(0));
        Assert.That(result.PositiveMethods, Is.Not.Null);
        Assert.That(result.PositiveMethods, Is.Empty);
    }

    [Test]
    public async Task DetectCultureBothPositive()
    {
        var service = new OfflineChinaDetectService();

        var result = await service.Detect(DetectionMethod.Culture, new()
        {
            TargetCulture = CultureInfo.GetCultureInfo("zh-CN"),
            TargetUICulture = CultureInfo.GetCultureInfo("zh-CN")
        });

        Assert.That(result.Rank, Is.EqualTo(2));
        Assert.That(result.PositiveMethods, Is.Not.Null);
        Assert.That(result.PositiveMethods.Count == 1, Is.True);
        Assert.That(result.PositiveMethods.First(), Is.EqualTo(DetectionMethod.Culture));
    }

    [Test]
    public async Task DetectAllOfflineBothPositive()
    {
        var service = new OfflineChinaDetectService();

        var result = await service.Detect(DetectionMethod.AllOffline, new()
        {
            TargetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"),
            TargetCulture = CultureInfo.GetCultureInfo("zh-CN"),
            TargetUICulture = CultureInfo.GetCultureInfo("zh-CN")
        });

        Assert.That(result.Rank, Is.GreaterThanOrEqualTo(3));
        Assert.That(result.PositiveMethods, Is.Not.Null);
        Assert.That(result.PositiveMethods.Count, Is.GreaterThanOrEqualTo(2));
    }
}