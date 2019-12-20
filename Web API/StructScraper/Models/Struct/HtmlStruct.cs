using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using HtmlAgilityPack;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace StructScraper.Models.Struct
{
    public class HtmlStruct
    {
        public HtmlStruct(Uri uri, string schemaType)
        {
            this.uri = uri;
            this.schemaType = schemaType;
        }

        public async Task<StructResponse> Get()
        {
            try
            {
                object structure = new Object();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "StructScraper");
                    using (HttpResponseMessage response = await client.GetAsync(uri))
                    using (HttpContent content = response.Content)
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            return new StructResponse() { Url = uri.AbsoluteUri, StatusCode = response.StatusCode, Struct = null, ErrorMessage = response.ReasonPhrase };
                        }

                        var enc = response.Content.Headers.ContentType.CharSet;
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

                            IList<MicrodataParser.MicroObject> microObjects = MicrodataParser.Parser.Parse(result, schemaType, uri);

                            if (microObjects.Count > 0)
                            {
                                MicrodataParser.MicroObject mo = microObjects[0];
                                IDictionary<string, object> moDictionary = mo.ToDictionary();
                                string jsonResult = JsonConvert.SerializeObject(moDictionary);
                                structure = JsonConvert.DeserializeObject(jsonResult);
                            }
                            else
                            {
                                IList<JObject> jsonldObjects = JsonLdParser.Parser.Parse(result, schemaType);
                                if (jsonldObjects.Count > 0)
                                {
                                    structure = jsonldObjects[0];

                                }

                            }


                        }
                    }
                    return new StructResponse() { Url = uri.AbsoluteUri, StatusCode = HttpStatusCode.OK, Struct = structure, ErrorMessage = null };
                }
            }
            catch (Exception e)
            {
                return new StructResponse() { Url = uri.AbsoluteUri, StatusCode = HttpStatusCode.BadRequest, Struct = null, ErrorMessage = e.Message };
            }


        }



        protected Uri uri;
        protected string schemaType;
    }
}