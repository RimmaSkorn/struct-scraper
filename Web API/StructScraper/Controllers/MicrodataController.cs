using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using StructScraper.Models.Microdata;


namespace StructScraper.Controllers
{
    public class MicrodataController : ApiController
    {
        [HttpPost]
        [ActionName("single-uri")]
        public async Task<IHttpActionResult> GetOrganization(MicrodataRequest request)
        {
            MicrodataResponse response = await request.GetResponse();

            if (response.Microdata == null)
            {
                HttpResponseMessage respMess = new HttpResponseMessage(response.StatusCode) { ReasonPhrase = response.ErrorMessage };
                return ResponseMessage(respMess);
            }
            return Ok(response.Microdata);
        }

        [HttpPost]
        [ActionName("multi-uri")]
        public async Task<IEnumerable<MicrodataResponse>> GetMicrodata(IEnumerable<MicrodataRequest> requests)
        {
            if (requests == null)
            {
                return new List<MicrodataResponse>();
            }

            var responses = await Task.WhenAll(requests.Select(async rq => await rq.GetResponse()));
            return responses;
        }

    }
}
