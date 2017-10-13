using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhackAMole.KubeAdmin.Models;
using WhackAMole.KubeAdmin.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WhackAMole.KubeAdmin.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PodsController : Controller
    {
        private readonly IAuthenticationProvider _auth;
        private readonly PodsRequest _podsRequest;

        public PodsController(IAuthenticationProvider authProvider)
        {
            _auth = authProvider;
            var k8s = new KubeRequestBuilder(_auth);
            _podsRequest = k8s.Create<PodsRequest>();
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Get("");
        } 

        // GET: api/values
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                
                var moles = new List<Pod>();

               
                var list = await _podsRequest.GetAllAsync(id);

                if (list == null || list.Length == 0)
                    return new OkObjectResult(moles);

                
                foreach (var pod in list)
                {
                    var starttime = DateTimeOffset.Parse(pod.Status.StartTime);
                    moles.Add(new Pod { Name = pod.MetaData.Name, Uid = pod.MetaData.Uid, Host = pod.Spec.NodeName, StartTime = starttime, Phase = pod.Status.Phase });
                }

                return new OkObjectResult(moles);
            }
            catch (Exception ex)
            {
                return new NotFoundResult();
            }
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
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _podsRequest.DeleteAsync(id);
            if (result)
                return new OkResult();

            return new NotFoundResult();
        }
    }
}
