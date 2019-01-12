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
        public HtmlMicrodata(Uri uri, string schemaType)
        {
            this.uri = uri;
            this.schemaType = schemaType;
        }

        public async Task<MicrodataResponse> Get()
        {
            try
            {
                object microdata = new Object();

                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(uri))
                using (HttpContent content = response.Content)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return new MicrodataResponse() { Url = uri.AbsoluteUri, StatusCode = response.StatusCode, Microdata = null, ErrorMessage = response.ReasonPhrase };
                    }

                    string result = await content.ReadAsStringAsync();
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        var doc = new HtmlDocument();

                        var enc = response.Content.Headers.ContentType.CharSet;
                        if (enc == null)
                        {
                            var ench = doc.DetectEncodingHtml(result);
                            byte[] rbytes = await content.ReadAsByteArrayAsync();
                            result = ench.GetString(rbytes);
                        }

                        IList<MicroObject> microObjects = Parser.Parse(result, schemaType);

                        if (microObjects.Count > 0)
                        {
                            MicroObject mo = microObjects[0];
                            IDictionary<string, object> moDictionary = mo.ToDictionary();
                            string jsonResult = JsonConvert.SerializeObject(moDictionary);
                            microdata = JsonConvert.DeserializeObject(jsonResult);
                        }


                    }
                }
                return new MicrodataResponse() { Url = uri.AbsoluteUri, StatusCode = HttpStatusCode.OK, Microdata = microdata, ErrorMessage = null };
            }
            catch (Exception e)
            {
                return new MicrodataResponse() { Url = uri.AbsoluteUri, StatusCode = HttpStatusCode.BadRequest, Microdata = null, ErrorMessage = e.Message };
            }


        }



        protected Uri uri;
        protected string schemaType;
    }
}