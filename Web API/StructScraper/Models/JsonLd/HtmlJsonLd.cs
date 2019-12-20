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

        public HtmlJsonLd(Uri uri, string schemaType)
        {
            this.uri = uri;
            this.schemaType = schemaType;
        }

        public async Task<JsonLdResponse> Get()
        {
            try
            {
                object jsonlddata = new Object();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "StructScraper");
                    using (HttpResponseMessage response = await client.GetAsync(uri))
                    using (HttpContent content = response.Content)
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            return new JsonLdResponse() { Url = uri.AbsoluteUri, StatusCode = response.StatusCode, JsonLd = null, ErrorMessage = response.ReasonPhrase };
                        }

                        string result = await content.ReadAsStringAsync();
                        if (!String.IsNullOrWhiteSpace(result))
                        {
                            HtmlDocument doc = new HtmlDocument();

                            var enc = response.Content.Headers.ContentType.CharSet;
                            if (enc == null)
                            {
                                var ench = doc.DetectEncodingHtml(result);
                                if (ench != null)
                                {
                                    byte[] rbytes = await content.ReadAsByteArrayAsync();
                                    result = ench.GetString(rbytes);
                                }
                            }

                            IList<JObject> jsonldObjects = Parser.Parse(result, schemaType);
                            if (jsonldObjects.Count > 0)
                            {
                                jsonlddata = jsonldObjects[0];

                            }


                        }
                    }

                }
                return new JsonLdResponse() { Url = uri.AbsoluteUri, StatusCode = HttpStatusCode.OK, JsonLd = jsonlddata, ErrorMessage = null };
            }
            catch (Exception e)
            {
                return new JsonLdResponse() { Url = uri.AbsoluteUri, StatusCode = HttpStatusCode.BadRequest,  JsonLd= null, ErrorMessage = e.Message };
            }


        }

        protected Uri uri;
        protected string schemaType;

    }
}