using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using StructScraper.Models.Struct;

namespace StructScraper.Controllers
{
    public class StructController : ApiController
    {
        [HttpPost]
        [ActionName("single-uri")]
        public async Task<IHttpActionResult> GetStruct(StructRequest request)
        {
            StructResponse response = await request.GetResponse();

            if (response.Struct == null)
            {
                HttpResponseMessage respMess = new HttpResponseMessage(response.StatusCode) { ReasonPhrase = response.ErrorMessage };
                return ResponseMessage(respMess);
            }
            return Ok(response.Struct);
        }

        [HttpPost]
        [ActionName("multi-uri")]
        public async Task<IEnumerable<StructResponse>> GetStruct(IEnumerable<StructRequest> requests)
        {
            if (requests == null)
            {
                return new List<StructResponse>();
            }

            var responses = await Task.WhenAll(requests.Select(async rq => await rq.GetResponse()));
            return responses;
        }


    }
}