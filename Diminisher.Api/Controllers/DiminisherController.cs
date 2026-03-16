using Application.Abstractions;
using Application.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Diminisher.Controllers;

[ApiController]
[Route("api/diminisher")]
public class DiminisherController(IUrlService urlService) : ControllerBase
{
    [HttpGet("/r/{code}")]
    public async Task<IActionResult> RedirectUrl(string code, CancellationToken ct)
    {
        var url = await urlService.ResolveAsync(code, ct);

        if (url is null)
            return NotFound();

        return Redirect(url);
    }
    
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await urlService.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateShortUrlDto dto, CancellationToken ct)
    {
        var result = await urlService.CreateAsync(dto, ct);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await urlService.DeleteAsync(id, ct);

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLongUrlDto url, CancellationToken ct)
    {
        await urlService.UpdateAsync(id, url, ct);

        return NoContent();
    }
    
}