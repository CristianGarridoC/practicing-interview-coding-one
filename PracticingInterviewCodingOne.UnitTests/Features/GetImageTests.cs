using System.Net;
using System.Text;
using ApplicationCore.Features.GetImage;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using RichardSzalay.MockHttp;
using Xunit;

namespace PracticingInterviewCodingOne.UnitTests.Features;

public class GetImageTests
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GetImageController _sut;

    public GetImageTests()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _sut = new GetImageController(_httpClientFactory);
    }

    [Fact]
    public async Task GetImage_ShouldReturnASuccessfulResponse_WhenRequestIsValid()
    {
        //Arrange
        var url = "https://picsum.photos/";
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var content = new MemoryStream(Encoding.UTF8.GetBytes("test"));
        mockHttpMessageHandler
            .When($"{url}*")
            .Respond(HttpStatusCode.OK, "image/jpeg", content);
        var httpClient = mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri(url);
        _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        //Act
        var result = await _sut.Get();

        //Assert
        result.As<FileContentResult>().ContentType.Should().Be("image/jpeg");
        Encoding.UTF8.GetString(result.As<FileContentResult>().FileContents).Should().Be("test");
    }

    [Fact]
    public async Task GetImage_ShouldReturnNotFound_WhenImageServiceIsDown()
    {
        //Arrange
        var url = "https://picsum.photos/";
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        mockHttpMessageHandler
            .When($"{url}*")
            .Respond(HttpStatusCode.InternalServerError);
        var httpClient = mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri(url);
        _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        //Act
        var result = await _sut.Get();

        //Assert
        result.As<NotFoundResult>().StatusCode.Should().Be(404);
    }
}