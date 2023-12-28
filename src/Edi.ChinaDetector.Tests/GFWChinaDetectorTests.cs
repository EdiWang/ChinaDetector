using NUnit.Framework;

namespace Edi.ChinaDetector.Tests;

[TestFixture]
public class GFWChinaDetectorTests
{
    [Test]
    public void Detect_Should_Not_Blow()
    {
        var handler = new HttpClientHandler
        {
            UseProxy = false
        };

        var httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        var detector = new GFWChinaDetector(httpClient);

        Assert.DoesNotThrowAsync(async () =>
        {
            int rank = await detector.Detect();
        });
    }
}