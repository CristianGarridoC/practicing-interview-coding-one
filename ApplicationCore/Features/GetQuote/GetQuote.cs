using System.Net;
using System.Net.Http.Json;
using System.Text;
using ApplicationCore.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using PracticingInterviewCodingOne.Dtos;

namespace ApplicationCore.Features.GetQuote;

[ApiController]
[Route("get-quote")]
public class GetQuoteController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public GetQuoteController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string key)
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
}