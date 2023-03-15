using System.Text;
using ApplicationCore.Entities;
using ApplicationCore.Features.ReadCsv;
using ApplicationCore.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace PracticingInterviewCodingOne.UnitTests.Features;

public class ReadCsvTests
{
    private readonly ReadCsvController _sut;

    public ReadCsvTests()
    {
        _sut = new ReadCsvController();
    }

    [Fact]
    public async Task Post_ShouldReturnOk_WhenFileIsValid()
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

        var result = await _sut.Post(file);
        result.As<OkObjectResult>().StatusCode.Should().Be(200);
        result.As<OkObjectResult>().Value.As<List<Person>>().Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenFileIsInvalid()
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

        var result = await _sut.Post(formFile);
        result.As<BadRequestObjectResult>().StatusCode.Should().Be(400);
        result.As<BadRequestObjectResult>().Value.As<ErrorResponse>().Message.Should().Contain("File is not valid");
    }

    [Fact]
    public async Task Post_ShouldReturn500Error_WhenFileIsInvalid()
    {
        var file = Substitute.For<IFormFile>();
        file.OpenReadStream().Throws(new Exception());
        var result = () => _sut.Post(file);
        await result.Should().ThrowAsync<Exception>();
    }
}