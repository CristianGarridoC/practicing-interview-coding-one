using System.Net;
using ApplicationCore.Entities;
using ApplicationCore.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationCore.Features.ReadCsv;

[ApiController]
[Route("read-csv")]
public class ReadCsvController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync();
        var lines = content.Split(Environment.NewLine);
        var contentFile = new List<Person>();
        var fistLine = true;
        foreach (var line in lines)
        {
            if (fistLine)
            {
                fistLine = false;
                continue;
            }
            var columns = line.Split(",");
            if (columns.Length != 3)
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "File is not valid"
                });
            }
            var person = new Person
            {
                FirstName = columns[0],
                LastName = columns[1],
                PhoneNumber = columns[2]
            };
            contentFile.Add(person);
        }

        return Ok(contentFile);
    }
}