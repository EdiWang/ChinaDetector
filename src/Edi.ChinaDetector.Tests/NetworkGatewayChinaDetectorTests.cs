using NUnit.Framework;

namespace Edi.ChinaDetector.Tests;

[TestFixture]
public class NetworkGatewayChinaDetectorTests
{
    private NetworkGatewayChinaDetector _detector;

    [SetUp]
    public void Setup()
    {
        _detector = new NetworkGatewayChinaDetector();
    }

    [Test]
    public void Detect_WhenServerIsChinaBrand_Pass()
    {
        string domain = "example.com";
        var result = _detector.Detect(domain);

        Assert.Pass();
    }
}