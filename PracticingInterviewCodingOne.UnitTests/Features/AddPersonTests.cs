using ApplicationCore.Entities;
using ApplicationCore.Features.AddPerson;
using ApplicationCore.Interfaces;
using ApplicationCore.Responses;
using FluentAssertions;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace PracticingInterviewCodingOne.UnitTests.Features;

public class AddPersonTests
{
    private readonly IPersonRepository _personRepository;
    private readonly IValidator<AddPersonRequest> _validator;
    private readonly AddPersonController _sut;
    
    public AddPersonTests()
    {
        _personRepository = Substitute.For<IPersonRepository>();
        _validator = Substitute.For<IValidator<AddPersonRequest>>();
        _sut = new AddPersonController(_validator, _personRepository);
    }
    
    [Fact]
    public async Task Post_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var person = new AddPersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "55555555"
        };
        _personRepository.SavePerson(Arg.Any<Person>()).Returns(Result.Ok(1));
        _validator.ValidateAsync(Arg.Any<AddPersonRequest>()).Returns(new ValidationResult());

        // Act
        var result = await _sut.Post(person);
        
        // Assert
        result.As<OkObjectResult>().StatusCode.Should().Be(200);
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(new
        {
            Id = 1
        });
    }
    
    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var person = new AddPersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "55555555"
        };
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ("FirstName", "First name is required")
        });
        _validator.ValidateAsync(Arg.Any<AddPersonRequest>()).Returns(validationResult);

        // Act
        var result = await _sut.Post(person);
        
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
    public async Task Post_ShouldReturnBadRequest_WhenSavePersonFails()
    {
        // Arrange
        var person = new AddPersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "55555555"
        };
        _personRepository.SavePerson(Arg.Any<Person>()).Returns(Result.Fail("Error"));
        _validator.ValidateAsync(Arg.Any<AddPersonRequest>()).Returns(new ValidationResult());

        // Act
        var result = await _sut.Post(person);
        
        // Assert
        result.As<BadRequestObjectResult>().StatusCode.Should().Be(400);
        result.As<BadRequestObjectResult>().Value.As<ErrorResponse>().Message.Should().Be("Error");
    }
}