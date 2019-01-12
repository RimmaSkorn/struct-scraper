using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using StructScraper.Models.Metadata;

namespace StructScraper.Controllers
{
    public class MetadataController : ApiController
    {

        [HttpPost]
        [ActionName("single-uri")]
        public async Task<IHttpActionResult> GetMetadata(MetadataRequest request)
        {
            MetadataResponse response = await request.GetResponse();

            if (response.Metadata == null)
            {
                HttpResponseMessage respMess = new HttpResponseMessage(response.StatusCode) { ReasonPhrase = response.ErrorMessage };
                return ResponseMessage(respMess);
            }
            return Ok(response.Metadata);
        }

        [HttpPost]
        [ActionName("multi-uri")]
        public async Task<IEnumerable<MetadataResponse>> GetMetadata(IEnumerable<MetadataRequest> requests)
        {
            if (requests == null)
            {
                return new List<MetadataResponse>();
            }

            var responses = await Task.WhenAll(requests.Select(async rq => await rq.GetResponse()));
            return responses;
        }

    }
}