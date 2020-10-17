using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stl.Fusion.Server;
using Stl.Serialization;

namespace Stl.Fusion.Tests.Services
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StringKeyValueController : FusionController
    {
        protected IKeyValueService<string> Service { get; }

        public StringKeyValueController(IKeyValueService<string> service) => Service = service;

        [HttpGet("{key?}")]
        public Task<Option<string>> TryGetAsync(string? key)
            => PublishAsync(ct => Service.TryGetAsync(key ?? "", ct));

        [HttpGet("{key?}")]
        public async Task<JsonString> GetAsync(string? key)
            => await PublishAsync(ct => Service.GetAsync(key ?? "", ct));

        [HttpPost("{key?}")]
        public async Task SetAsync(string? key)
        {
            var cancellationToken = HttpContext.RequestAborted;
            using var reader = new StreamReader(Request.Body);
            var value = await reader.ReadToEndAsync();
            await Service.SetAsync(key ?? "", value ?? "", cancellationToken);
        }

        [HttpGet("{key?}")]
        public Task RemoveAsync(string? key)
            => Service.RemoveAsync(key ?? "", HttpContext.RequestAborted);
    }
}
