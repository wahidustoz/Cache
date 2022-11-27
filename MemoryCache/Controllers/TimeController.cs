using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace MemoryCache.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeController : ControllerBase
{
    [HttpGet("fresh")]
    public IActionResult Fresh()
        => Ok(new { time = GetTime(), host = Dns.GetHostName() });

    [HttpGet("every-5-seconds")]
    public async Task<IActionResult> Every5SecondsAsync([FromServices] IMemoryCache cache)
    {
        var key = "every-5-seconds";
        var time = await cache.GetOrCreateAsync(key, (entry) =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);

            return Task.FromResult(GetTime());
        });

        return Ok(new { time = time, host = Dns.GetHostName() });
    }

    [HttpGet("use-it-or-lose-it")]
    public async Task<IActionResult> UseItOrLoseItAsync([FromServices] IMemoryCache cache)
    {
        var key = "use-it-or-lose-it";
        var time = await cache.GetOrCreateAsync(key, (cacheEntry) =>
        {
            cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(2);
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            return Task.FromResult(GetTime());
        });

        return Ok(new { time = time, host = Dns.GetHostName() });
    }


    private string GetTime() => DateTime.Now.ToString("HH:mm:ss");
}