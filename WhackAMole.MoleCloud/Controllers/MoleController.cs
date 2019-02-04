using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WhackAMole.MoleCloud.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WhackAMole.MoleCloud.Controllers
{
    [Route("api/[controller]")]
    public class MoleController : Controller
    {
        private const int START = 65;
        private const int END = 91;


        private int _count = START;

        public MoleController(IOptions<MoleSettings> molesettings)
        {
            HEX_COLOR = molesettings.Value.Color;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var pod = Environment.GetEnvironmentVariable("POD_NAME");
            var host = Environment.GetEnvironmentVariable("NODE_NAME");
            var rnd = new Random();
            var c = (char)rnd.Next(START, END);
            var character = c.ToString();
            var mole = new MoleState() { Name = $"{pod}", CurrentChar = character, Color = HEX_COLOR};
            

            return new OkObjectResult(mole);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
