using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using StructScraper.Models.JsonLd;

namespace StructScraper.Controllers
{
        public class JsonLdController : ApiController
        {
            [HttpPost]
            [ActionName("single-uri")]
            public async Task<IHttpActionResult> GetOrganization(JsonLdRequest request)
            {
                JsonLdResponse response = await request.GetResponse();

                if (response.JsonLd == null)
                {
                    HttpResponseMessage respMess = new HttpResponseMessage(response.StatusCode) { ReasonPhrase = response.ErrorMessage };
                    return ResponseMessage(respMess);
                }
                return Ok(response.JsonLd);
            }

            [HttpPost]
            [ActionName("multi-uri")]
            public async Task<IEnumerable<JsonLdResponse>> GetJsonLd(IEnumerable<JsonLdRequest> requests)
            {
                if (requests == null)
                {
                    return new List<JsonLdResponse>();
                }

                var responses = await Task.WhenAll(requests.Select(async rq => await rq.GetResponse()));
                return responses;
            }


        }
    }