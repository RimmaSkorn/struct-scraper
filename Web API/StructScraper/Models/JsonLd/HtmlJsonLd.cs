using System;
using System.Collections.Generic;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

using JsonLdParser;

namespace StructScraper.Models.JsonLd
{
    public class HtmlJsonLd
    {

        public HtmlJsonLd(Resource resource, string schemaType)
        {
            this.resource = resource;
            this.schemaType = schemaType;
        }

        public async Task<JsonLdResponse> Get()
        {
            try
            {
                using (HttpResponseMessage response = await resource.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return new JsonLdResponse() { Url = resource.Url, StatusCode = response.StatusCode, JsonLd = null, ErrorMessage = response.ReasonPhrase };
                    }

                    using (HttpContent content = response.Content)
                    {
                        return await Get(content);

                    }
                }
            }
            catch (Exception e)
            {
                return new JsonLdResponse() { Url = resource.Url, StatusCode = HttpStatusCode.BadRequest, JsonLd = null, ErrorMessage = e.Message };

            }
        }

        public async Task<JsonLdResponse> Get(HttpContent content)
        {
            try
            {

                object jsonlddata = new Object();
                var enc = content.Headers.ContentType.CharSet;
                string result = await content.ReadAsStringAsync();

                if (!String.IsNullOrWhiteSpace(result))
                {
                    var doc = new HtmlDocument();

                    if (enc == null)
                    {
                        var ench = doc.DetectEncodingHtml(result);
                        byte[] rbytes = await content.ReadAsByteArrayAsync();
                        result = ench.GetString(rbytes);
                    }

                    IList<JObject> jsonldObjects = Parser.Parse(result, schemaType);
                    if (jsonldObjects.Count > 0)
                    {
                        jsonlddata = jsonldObjects[0];

                    }

                }
                return new JsonLdResponse() { Url = resource.Url, StatusCode = HttpStatusCode.OK, JsonLd = jsonlddata, ErrorMessage = null };
            }
            catch (Exception e)
            {
                return new JsonLdResponse() { Url = resource.Url, StatusCode = HttpStatusCode.BadRequest, JsonLd = null, ErrorMessage = e.Message };
            }
        }

        protected Resource resource;
        protected string schemaType;

    }
}