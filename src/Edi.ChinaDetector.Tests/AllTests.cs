﻿using System.Globalization;
using Moq;
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
        var httpClient = new HttpClient();
        var service = new ChinaDetectService(httpClient);

        var result = await service.Detect(DetectionMethod.TimeZone, new()
        {
            TargetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
        });

        Assert.That(result.Rank, Is.EqualTo(0));
        Assert.That(result.PositiveMethods, Is.Not.Null);
        Assert.That(result.PositiveMethods, Is.Empty);
    }

    [Test]
    public async Task DetectCultureOnePositive()
    {
        var httpClient = new HttpClient();
        var service = new ChinaDetectService(httpClient);

        var result = await service.Detect(DetectionMethod.Culture, new()
        {
            TargetCulture = CultureInfo.GetCultureInfo("zh-CN"),
            TargetUICulture = CultureInfo.GetCultureInfo("en-US")
        });

        Assert.That(result.Rank, Is.EqualTo(1));
    }
}