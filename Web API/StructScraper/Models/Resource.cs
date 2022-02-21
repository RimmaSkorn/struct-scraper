using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

using StructScraper.Models.Metadata;
using StructScraper.Models.Microdata;
using StructScraper.Models.JsonLd;
using StructScraper.Models.Struct;


namespace StructScraper.Models
{
    public enum ResourceType { Html, Pdf, Docx, Doc, Unsupported }

    public class Resource
    {
        public string Url { get; set; }

        public async Task<MetadataResponse> GetMetadata(IEnumerable<string> metaNames)
        {
            try
            {
                ResourceMetadata metadata = new ResourceMetadata(this, metaNames);
                return await metadata.Get();
            }
            catch (Exception e)
            {
                return new MetadataResponse() { Url = Url, StatusCode = HttpStatusCode.BadRequest, Metadata = null, ErrorMessage = e.Message };
            }

        }

        public async Task<MicrodataResponse> GetMicrodata(string schemaType)
        {
            try
            {
                HtmlMicrodata microdata = new HtmlMicrodata(this, schemaType);
                return await microdata.Get();
            }
            catch (Exception e)
            {
                return new MicrodataResponse() { Url = Url, StatusCode = HttpStatusCode.BadRequest, Microdata = null, ErrorMessage = e.Message };
            }
        }

        public async Task<JsonLdResponse> GetJsonLd(string schemaType)
        {
            try
            {

                HtmlJsonLd jsonld = new HtmlJsonLd(this, schemaType);
                return await jsonld.Get();
            }
            catch (Exception e)
            {
                return new JsonLdResponse() { Url = Url, StatusCode = HttpStatusCode.BadRequest, JsonLd = null, ErrorMessage = e.Message };
            }
        }

        public async Task<StructResponse> GetStruct(string schemaType)
        {
            try
            {
                HtmlStruct structure = new HtmlStruct(this, schemaType);
                return await structure.Get();
            }
            catch (Exception e)
            {
                return new StructResponse() { Url = Url, StatusCode = HttpStatusCode.BadRequest, Struct = null, ErrorMessage = e.Message };
            }
        }

        private static ResourceType GetType(string contentType)
        {
            if (contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return ResourceType.Pdf;
            }
            else if (contentType.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document", StringComparison.OrdinalIgnoreCase))
            {
                return ResourceType.Docx;
            }
            else if (contentType.Equals("application/msword", StringComparison.OrdinalIgnoreCase))
            {
                return ResourceType.Doc;
            }
            else if (contentType.Equals("text/html", StringComparison.OrdinalIgnoreCase))
            {
                return ResourceType.Html;
            }
            else
            {
                return ResourceType.Unsupported;
            }
        }

        public static ResourceType GetType(HttpContent content)
        {
            return GetType(content.Headers.ContentType.MediaType);
        }

        public async Task<HttpResponseMessage> GetResponse()
        {
            Uri uri = new UriBuilder(Url).Uri;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "StructScraper");
                return await client.GetAsync(uri);
            }
        }

    }
}