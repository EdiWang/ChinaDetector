using System.Net;
using Moq;
using NUnit.Framework;
using Moq.Protected;

namespace Edi.ChinaDetector.Tests;

[TestFixture]
public class IPChinaDetectorTests
{
    private MockRepository _mockRepository;
    private Mock<HttpMessageHandler> _handlerMock;
    private HttpClient _magicHttpClient;

    private IPChinaDetector _ipChinaDetector;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new(MockBehavior.Default);
        _handlerMock = _mockRepository.Create<HttpMessageHandler>();

        _magicHttpClient = new(_handlerMock.Object);
        _ipChinaDetector = new IPChinaDetector(_magicHttpClient);
    }

    [Test]
    public async Task Detect_Should_Return_Rank_And_IPAddress()
    {
        // Arrange
        var expectedRank = 1;
        var expectedIPAddress = "192.168.0.1";

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \n    \"query\" : \"192.168.0.1\",\n    \"CountryCode\" : \"CN\"\n}")
            })
            .Verifiable();

        // Act
        var (rank, ipAddress) = await _ipChinaDetector.Detect();

        // Assert
        Assert.That(expectedRank, Is.EqualTo(rank));
        Assert.That(expectedIPAddress, Is.EqualTo(ipAddress));
    }

    [Test]
    public async Task Detect_Should_Return_Rank_1_And_Null_IPAddress_When_HttpRequestException_Occurs()
    {
        // Arrange
        var expectedRank = 1;
        string expectedIPAddress = null;

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException())
            .Verifiable();

        // Act
        var (rank, ipAddress) = await _ipChinaDetector.Detect();

        // Assert
        Assert.That(expectedRank, Is.EqualTo(rank));
        Assert.That(expectedIPAddress, Is.EqualTo(ipAddress));
    }
}