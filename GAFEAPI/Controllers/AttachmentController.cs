using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GAFEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        // GET: api/<AttachmentController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AttachmentController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read);
            
            return stream.Length.ToString();
        }

        // POST api/<AttachmentController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AttachmentController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AttachmentController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
