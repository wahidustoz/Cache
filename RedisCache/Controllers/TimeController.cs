using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedisCache.Extensions;
using System.Net;

namespace RedisCache.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TimeController : ControllerBase
    {
        [HttpGet("fresh")]
        public IActionResult Fresh()
        => Ok(new { time = GetTime(), host = Dns.GetHostName() });

        [HttpGet("every-5-seconds")]
        public async Task<IActionResult> Every5SecondsAsync([FromServices] IDistributedCache cache)
        {
            var key = "every-5-seconds";
            var time = await cache.GetOrCreateStringAsync(key, (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);

                return Task.FromResult(GetTime());
            });

            return Ok(new { time = time, host = Dns.GetHostName() });
        }

        [HttpGet("use-it-or-lose-it")]
        public async Task<IActionResult> UseItOrLoseItAsync([FromServices] IDistributedCache cache)
        {
            var key = "use-it-or-lose-it";
            var time = await cache.GetOrCreateStringAsync(key, (cacheEntry) =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(2);
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                return Task.FromResult(GetTime());
            });

            return Ok(new { time = time, host = Dns.GetHostName() });
        }


        private string GetTime() => DateTime.Now.ToString("HH:mm:ss");
    }
}
