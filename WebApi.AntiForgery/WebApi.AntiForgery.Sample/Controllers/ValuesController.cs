using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.AntiForgery.Sample.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public string Get()
        {
            return "Test GET";
        }

        // POST api/values
        [ValidateAntiForgeryToken]
        public string Post()
        {
            return "Test POST";
        }

        // PUT api/values
        [ValidateAntiForgeryToken]
        public string Put()
        {
            return "Test PUT";
        }

        // DELETE api/values
        [ValidateAntiForgeryToken]
        public string Delete()
        {
            return "Test DELETE";
        }
    }
}
