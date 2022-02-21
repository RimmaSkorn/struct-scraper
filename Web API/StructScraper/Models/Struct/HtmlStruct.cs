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
        public HtmlStruct(Resource resource, string schemaType)
        {
            this.resource = resource;
            this.schemaType = schemaType;
        }

        protected Resource resource;
        protected string schemaType;

        public async Task<StructResponse> Get()
        {
            try
            {
                using (HttpResponseMessage response = await resource.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return new StructResponse() { Url = resource.Url, StatusCode = response.StatusCode, Struct = null, ErrorMessage = response.ReasonPhrase };
                    }

                    using (HttpContent content = response.Content)
                    {
                        return await Get(content);

                    }
                }
            }
            catch (Exception e)
            {
                return new StructResponse() { Url = resource.Url, StatusCode = HttpStatusCode.BadRequest, Struct = null, ErrorMessage = e.Message };

            }
        }

        public async Task<StructResponse> Get(HttpContent content)
        {
            try
            {

                object structure = new Object();
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

                    IList<MicrodataParser.MicroObject> microObjects = MicrodataParser.Parser.Parse(result, schemaType, new Uri(resource.Url));

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
                return new StructResponse() { Url = resource.Url, StatusCode = HttpStatusCode.OK, Struct = structure, ErrorMessage = null };
            }
            catch (Exception e)
            {
                return new StructResponse() { Url = resource.Url, StatusCode = HttpStatusCode.BadRequest, Struct = null, ErrorMessage = e.Message };
            }
        }

    }



}
