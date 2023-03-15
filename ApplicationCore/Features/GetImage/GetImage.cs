using Microsoft.AspNetCore.Mvc;

namespace ApplicationCore.Features.GetImage;

[ApiController]
[Route("get-image")]
public class GetImage : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GetImage(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Get()
    {
        using var client = _httpClientFactory.CreateClient("picsum");
        var response = await client.GetAsync("200/300.jpg");
        var content = await response.Content.ReadAsByteArrayAsync();
        if (content.Length == 0)
        {
            return NotFound();
        }
        return File(content, "image/jpeg");
    }
}