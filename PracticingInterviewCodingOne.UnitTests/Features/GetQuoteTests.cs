using ApplicationCore.Features.GetQuote;
using ApplicationCore.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSubstitute;
using PracticingInterviewCodingOne.Dtos;
using RichardSzalay.MockHttp;
using Xunit;

namespace PracticingInterviewCodingOne.UnitTests.Features;

public class GetQuoteTests
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GetQuoteController _sut;

    public GetQuoteTests()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _sut = new GetQuoteController(_httpClientFactory);
    }
    
    [Fact]
    public async Task GetQuote_ShouldReturnASuccessfulResponse_WhenRequestIsValid()
    {
        //Arrange
        var expectedResult = new QuoteResponse
        {
            QuoteAuthor = "John Doe",
            QuoteText = "This is a quote"
        };
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        mockHttpMessageHandler
            .When("https://api.forismatic.com/api/1.0/*")
            .Respond("application/json", JsonConvert.SerializeObject(expectedResult));
        var httpClient = mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.forismatic.com/");
        _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        //Act
        var result = await _sut.Get("");

        //Assert
        result.As<OkObjectResult>().StatusCode.Should().Be(200);
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async Task GetQuote_ShouldReturnBadRequest_WhenKeyIsInvalid()
    {
        //Arrange
        var key = "1234567";

        //Act
        var result = await _sut.Get(key);

        //Assert
        result.As<BadRequestObjectResult>().StatusCode.Should().Be(400);
        result.As<BadRequestObjectResult>().Value.As<ErrorResponse>().Message.Should().Be("Key is too long");
        _httpClientFactory.DidNotReceive().CreateClient(Arg.Any<string>());
    }
}