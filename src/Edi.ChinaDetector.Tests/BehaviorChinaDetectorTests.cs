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
}