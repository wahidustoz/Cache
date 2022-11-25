using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MemoryCache.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeController : ControllerBase
{
    [HttpGet("[action]")]
    public IActionResult Fresh()
       => Ok(new { time = GetTimeAndIP() });

    [HttpGet("every-5-seconds")]
    public async Task<IActionResult> Every5SecondsAsync([FromServices] IMemoryCache cache)
    {
        var key = $"time:every-5-seconds";
        var time = await cache.GetOrCreateAsync<string>(key, (cacheEntry) =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
            return Task.FromResult(GetTimeAndIP());
        });

        return Ok(time);
    }

    [HttpGet("use-it-or-lose-it")]
    public async Task<IActionResult> UseItOrLoseItAsync([FromServices] IMemoryCache cache)
    {
        var key = $"time:use-it-or-lose-it";
        var time = await cache.GetOrCreateAsync<string>(key, (cacheEntry) =>
        {
            cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(1);
            return Task.FromResult(GetTimeAndIP());
        });

        return Ok(time);
    }

    private string GetTimeAndIP() => $"{DateTime.Now.ToString("yyyyy/MM/dd HH:mm:ss")} @ { System.Net.Dns.GetHostName() }";
}
