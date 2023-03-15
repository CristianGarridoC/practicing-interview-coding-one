/*using System.Net;
using System.Text;
using FluentAssertions;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PracticingInterviewCodingOne.Controllers;
using PracticingInterviewCodingOne.Dtos;
using PracticingInterviewCodingOne.Entities;
using PracticingInterviewCodingOne.Interfaces;
using RichardSzalay.MockHttp;
using Xunit;

namespace PracticingInterviewCodingOne.UnitTests.Controllers;

public class InterviewControllerTests
{
    private readonly InterviewController _sut;
    private readonly IPersonRepository _personRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IValidator<PersonRequest> _validator;

    public InterviewControllerTests()
    {
        _personRepository = Substitute.For<IPersonRepository>();
        _validator = Substitute.For<IValidator<PersonRequest>>();
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _sut = new InterviewController(_personRepository, _httpClientFactory);
    }
    
    [Fact]
    public async Task Person_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var person = new PersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "55555555"
        };
        _personRepository.SavePerson(Arg.Any<Person>()).Returns(Result.Ok(1));
        _validator.ValidateAsync(Arg.Any<PersonRequest>()).Returns(new ValidationResult());

        // Act
        var result = await _sut.SavePerson(person, _validator);
        
        // Assert
        result.As<OkObjectResult>().StatusCode.Should().Be(200);
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(new
        {
            Id = 1
        });
    }
    
    [Fact]
    public async Task Person_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var person = new PersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "55555555"
        };
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ("FirstName", "First name is required")
        });
        _validator.ValidateAsync(Arg.Any<PersonRequest>()).Returns(validationResult);

        // Act
        var result = await _sut.SavePerson(person, _validator);
        
        // Assert
        result.As<BadRequestObjectResult>().StatusCode.Should().Be(400);
        result.As<BadRequestObjectResult>().Value.As<ErrorResponse>().Errors.Should().BeEquivalentTo(
            new Dictionary<string, string[]>
            {
                { "FirstName", new[] { "First name is required" } }
            }
        );
    }
    
    [Fact]
    public async Task Person_ShouldReturnBadRequest_WhenSavePersonFails()
    {
        // Arrange
        var person = new PersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "55555555"
        };
        _personRepository.SavePerson(Arg.Any<Person>()).Returns(Result.Fail("Error"));
        _validator.ValidateAsync(Arg.Any<PersonRequest>()).Returns(new ValidationResult());

        // Act
        var result = await _sut.SavePerson(person, _validator);
        
        // Assert
        result.As<BadRequestObjectResult>().StatusCode.Should().Be(400);
        result.As<BadRequestObjectResult>().Value.As<ErrorResponse>().Message.Should().Be("Error");
    }

    [Fact]
    public async Task ReadFile_ShouldReturnOk_WhenFileIsValid()
    {
        // Arrange
        var expectedResult = new List<Person>
        {
            new Person { FirstName = "John", LastName = "Smith", PhoneNumber = "1-310-475-5574" }
        };
        var content = "FirstName,LastName,PhoneNumber" + Environment.NewLine + "John,Smith,1-310-475-5574";
        var fileName = "test.txt";
        var parameterName = "file";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var file = new FormFile(stream, 0, stream.Length, parameterName, fileName);

        var result = await _sut.ReadFile(file);
        result.As<OkObjectResult>().StatusCode.Should().Be(200);
        result.As<OkObjectResult>().Value.As<List<Person>>().Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async Task ReadFile_ShouldReturnBadRequest_WhenFileIsInvalid()
    {
        // Arrange
        var content = "FirstName,LastName,PhoneNumber" + Environment.NewLine + "John,Smith";
        var formFile = Substitute.For<IFormFile>();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        formFile.OpenReadStream().Returns(stream);
        //var fileName = "test.txt";
        //var parameterName = "file";
        //var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        //var file = new FormFile(stream, 0, stream.Length, parameterName, fileName);

        var result = await _sut.ReadFile(formFile);
        result.As<BadRequestObjectResult>().StatusCode.Should().Be(400);
        result.As<BadRequestObjectResult>().Value.As<ErrorResponse>().Message.Should().Contain("File is not valid");
    }

    [Fact]
    public async Task ReadFile_ShouldReturn500Error_WhenFileIsInvalid()
    {
        var file = Substitute.For<IFormFile>();
        file.OpenReadStream().Throws(new Exception());
        var result = () => _sut.ReadFile(file);
        await result.Should().ThrowAsync<Exception>();
    }
    
    private static Stream CreateStream(string content)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        return stream;
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
        var result = await _sut.GetQuote("");

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
        var result = await _sut.GetQuote(key);

        //Assert
        result.As<BadRequestObjectResult>().StatusCode.Should().Be(400);
        result.As<BadRequestObjectResult>().Value.As<ErrorResponse>().Message.Should().Be("Key is too long");
        _httpClientFactory.DidNotReceive().CreateClient(Arg.Any<string>());
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
        var result = await _sut.GetImage();

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
        var result = await _sut.GetImage();

        //Assert
        result.As<NotFoundResult>().StatusCode.Should().Be(404);
    }

}*/