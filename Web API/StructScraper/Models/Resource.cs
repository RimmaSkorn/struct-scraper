using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

using StructScraper.Models.Metadata;
using StructScraper.Models.Microdata;
using StructScraper.Models.JsonLd;
using StructScraper.Models.Struct;


namespace StructScraper.Models
{
    public enum ResourceType { Html, Pdf, Docx, Doc }

    public class Resource
    {
        public string Url { get; set; }

        public async Task<MetadataResponse> GetMetadata(IEnumerable<string> metaNames)
        {
            try
            {
                Uri uri = new UriBuilder(Url).Uri;

                ResourceMetadata metadata;
                switch (resourceType)
                {
                    case ResourceType.Html:
                        metadata = new HtmlMetadata(uri, metaNames);
                        break;
                    case ResourceType.Pdf:
                        metadata = new PdfMetadata(uri, metaNames);
                        break;
                    case ResourceType.Docx:
                        metadata = new DocxMetadata(uri, metaNames);
                        break;
                    case ResourceType.Doc:
                        metadata = new DocMetadata(uri, metaNames);
                        break;
                    default:
                        metadata = new HtmlMetadata(uri, metaNames);
                        break;
                }
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
                Uri uri = new UriBuilder(Url).Uri;

                HtmlMicrodata microdata = new HtmlMicrodata(uri,schemaType);
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
                Uri uri = new UriBuilder(Url).Uri;

                HtmlJsonLd jsonld = new HtmlJsonLd(uri, schemaType);
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
                Uri uri = new UriBuilder(Url).Uri;

                HtmlStruct structure = new HtmlStruct(uri, schemaType);
                return await structure.Get();
            }
            catch (Exception e)
            {
                return new StructResponse() { Url = Url, StatusCode = HttpStatusCode.BadRequest, Struct = null, ErrorMessage = e.Message };
            }
        }

        private ResourceType resourceType
        {
            get
            {
                if (Url.EndsWith(".pdf", true, new System.Globalization.CultureInfo("en-US")))
                {
                    return ResourceType.Pdf;
                }
                else if (Url.EndsWith(".docx", true, new System.Globalization.CultureInfo("en-US")))
                {
                    return ResourceType.Docx;
                }
                else if (Url.EndsWith(".doc", true, new System.Globalization.CultureInfo("en-US")))
                {
                    return ResourceType.Doc;
                }
                else
                {
                    return ResourceType.Html;
                }

            }
        }
    }


}