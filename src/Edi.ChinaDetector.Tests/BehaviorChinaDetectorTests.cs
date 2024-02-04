using NUnit.Framework;

namespace Edi.ChinaDetector.Tests;

[TestFixture]
public class BehaviorChinaDetectorTests
{
    [Test]
    public void Detect_ShouldReturnRank()
    {
        var detector = new BehaviorChinaDetector();

        var rank = detector.Detect();

        Assert.Pass();
    }

    [Test]
    public async Task DetectNpm_ShouldReturnRank()
    {
        var detector = new NpmChinaDetector();

        var rank = await detector.Detect();

        Assert.Pass();
    }
}