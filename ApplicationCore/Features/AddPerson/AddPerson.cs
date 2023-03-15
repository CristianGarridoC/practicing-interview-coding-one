using System.Net;
using ApplicationCore.Entities;
using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using ApplicationCore.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationCore.Features.AddPerson;

[ApiController]
[Route("add-person")]
public class AddPersonController : ControllerBase
{
    private readonly IValidator<AddPersonRequest> _validator;
    private readonly IPersonRepository _personRepository;
    
    public AddPersonController(IValidator<AddPersonRequest> validator, IPersonRepository personRepository)
    {
        _validator = validator;
        _personRepository = personRepository;
    }
    
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Post([FromBody] AddPersonRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.FormatValidationErrors(HttpStatusCode.BadRequest));
        }

        var person = new Person
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber
        };
        var result = await _personRepository.SavePerson(person);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.FormatResultFailed(HttpStatusCode.BadRequest));
        }
        
        return Ok(new
        {
            Id = result.Value,
        });
    }
}

public sealed class AddPersonRequest
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string PhoneNumber { get; init; }
}

internal sealed class PersonRequestValidator : AbstractValidator<AddPersonRequest>
{
    public PersonRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required");
    }
}