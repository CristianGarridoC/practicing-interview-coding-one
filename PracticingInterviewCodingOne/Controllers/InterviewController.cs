using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using StreamReader = System.IO.StreamReader;

namespace PracticingInterviewCodingOne.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class InterviewController : ControllerBase
{
    /*private readonly IPersonRepository _personRepository;
    private readonly IHttpClientFactory _httpClientFactory;

    public InterviewController(
        IPersonRepository personRepository,
        IHttpClientFactory httpClientFactory)
    {
        _personRepository = personRepository;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> SavePerson(
        [FromBody] PersonRequest request,
        [FromServices] IValidator<PersonRequest> validator
    )
    {
        var validationResult = await validator.ValidateAsync(request);
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

    [HttpPost]
    [ProducesResponseType(typeof(List<PersonRequest>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> ReadFile(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync();
        var lines = content.Split(Environment.NewLine);
        var contentFile = new List<Person>();
        var counterLine = 0;
        foreach (var line in lines)
        {
            counterLine++;
            if (counterLine == 1)
            {
                continue;
            }
            var lineSplitted = line.Split(",");
            if (lineSplitted.Length != 3)
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = $"File is not valid, line {counterLine} is not valid"
                });
            }
            var person = new Person
            {
                FirstName = lineSplitted[0],
                LastName = lineSplitted[1],
                PhoneNumber = lineSplitted[2]
            };
            contentFile.Add(person);
        }

        return Ok(contentFile);
    }
    
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetQuote([FromQuery] string key)
    {
        if(!string.IsNullOrWhiteSpace(key) && key.Length > 6)
        {
            return BadRequest(Result.Fail("Key is too long").Errors.FormatResultFailed(HttpStatusCode.BadRequest));
        }
        using var client = _httpClientFactory.CreateClient("quote");
        var queryParameters = new StringBuilder();
        queryParameters.Append("?method=getQuote&format=json&lang=en");
        if(!string.IsNullOrWhiteSpace(key))
        {
            queryParameters.Append($"&key={key}");
        }
        var response = await client.GetAsync($"api/1.0/{queryParameters}"); 
        var content = await response.Content.ReadFromJsonAsync<QuoteResponse>();
        return Ok(content);
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetImage()
    {
        using var client = _httpClientFactory.CreateClient("picsum");
        var response = await client.GetAsync("200/300.jpg");
        var content = await response.Content.ReadAsByteArrayAsync();
        if (content.Length == 0)
        {
            return NotFound();
        }
        return File(content, "image/jpeg");
    }*/
}