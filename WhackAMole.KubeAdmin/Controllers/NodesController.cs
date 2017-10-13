using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhackAMole.KubeAdmin.Models;
using WhackAMole.KubeAdmin.Services;

namespace WhackAMole.KubeAdmin.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class NodesController : Controller
    {
        private readonly IAuthenticationProvider _auth;
        private readonly NodesRequest _nodesRequest;

        public NodesController(IAuthenticationProvider authProvider)
        {
            _auth = authProvider;
            var k8s = new KubeRequestBuilder(_auth);
            _nodesRequest = k8s.Create<NodesRequest>();
        }
        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var list = await _nodesRequest.GetAllAsync();

                if (list == null || list.Length == 0)
                    return new NotFoundObjectResult(null);

                var nodes = new List<Node>();
                foreach (var node in list)
                    nodes.Add(new Node { Name = node.MetaData.Name, Uid = node.MetaData.Uid });

                return new OkObjectResult(nodes);
            }
            catch (Exception ex)
            {
                return new NotFoundResult();
            }
        }



        // POST: api/Noes
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Noes/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
