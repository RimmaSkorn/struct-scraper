using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using HtmlAgilityPack;

using MicrodataParser;


namespace StructScraper.Models.Microdata
{
    public class HtmlMicrodata
    {
        public HtmlMicrodata(Resource resource, string schemaType)
        {
            this.resource = resource;
            this.schemaType = schemaType;
        }

        public async Task<MicrodataResponse> Get()
        {
            try
            {
                using (HttpResponseMessage response = await resource.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return new MicrodataResponse() { Url = resource.Url, StatusCode = response.StatusCode, Microdata = null, ErrorMessage = response.ReasonPhrase };
                    }

                    using (HttpContent content = response.Content)
                    {
                        return await Get(content);

                    }
                }
            }
            catch (Exception e)
            {
                return new MicrodataResponse() { Url = resource.Url, StatusCode = HttpStatusCode.BadRequest, Microdata = null, ErrorMessage = e.Message };

            }
        }

        public async Task<MicrodataResponse> Get(HttpContent content)
        {
            try
            {

                object microdata = new Object();

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

                    IList<MicroObject> microObjects = Parser.Parse(result, schemaType, new Uri(resource.Url));

                    if (microObjects.Count > 0)
                    {
                        MicroObject mo = microObjects[0];
                        IDictionary<string, object> moDictionary = mo.ToDictionary();
                        string jsonResult = JsonConvert.SerializeObject(moDictionary);
                        microdata = JsonConvert.DeserializeObject(jsonResult);
                    }

                }
                return new MicrodataResponse() { Url = resource.Url, StatusCode = HttpStatusCode.OK, Microdata = microdata, ErrorMessage = null };
            }
            catch (Exception e)
            {
                return new MicrodataResponse() { Url = resource.Url, StatusCode = HttpStatusCode.BadRequest, Microdata = null, ErrorMessage = e.Message };
            }
        }

        protected Resource resource;
        protected string schemaType;
    }
}