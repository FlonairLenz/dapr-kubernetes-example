using System.Threading.Tasks;
using Dapr;
using Dapr.Client;
using HelloWorld.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        private const string StoreName = "statestore";
        private readonly DaprClient daprClient;

        public HelloWorldController(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }

        [HttpPost("hello")]
        public async Task<IActionResult> PostMessage([FromBody] HelloWorldModel model)
        {
            await this.daprClient.SaveStateAsync(StoreName, model.Id, model);
            return Ok();
        }

        [HttpGet("hello/{key}")]
        public IActionResult GetMessage([FromState(StoreName, "key")] StateEntry<HelloWorldModel> model)
        {
            if (model == null) return NotFound();
            return Ok(model.Value);
        }
        
        [HttpGet("hello")]
        public async Task<IActionResult> GetMessageByQuery([FromQuery] string key)
        {
            var model = await this.daprClient.GetStateAsync<HelloWorldModel>(StoreName, key);
            return Ok(model);
        }
    }
}