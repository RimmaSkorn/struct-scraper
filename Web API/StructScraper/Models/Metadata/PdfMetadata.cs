using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

using iText.Kernel.Pdf;


namespace StructScraper.Models.Metadata
{
    public class PdfMetadata : ResourceMetadata
    {
        public PdfMetadata(Resource resource, IEnumerable<string> metaNames) : base(resource, metaNames) { }

        public override async Task<MetadataResponse> Get(HttpContent content)
        {
            try
            {
                IDictionary<string, string> metadata = new Dictionary<string, string>();

                Stream resultStream = await content.ReadAsStreamAsync();

                if (resultStream != null)
                {
                    PdfDocument doc = new PdfDocument(new PdfReader(resultStream));
                    PdfDocumentInfo info = doc.GetDocumentInfo();
                    foreach (string name in metaNames)
                    {
                        string value = info.GetMoreInfo(name);
                        if (!String.IsNullOrEmpty(value))
                        {
                            metadata[name] = value;
                        }
                    }
                }

                return new MetadataResponse() { Url = resource.Url, StatusCode = HttpStatusCode.OK, Metadata = metadata, ErrorMessage = null };


            }
            catch (Exception e)
            {
                return new MetadataResponse() { Url = resource.Url, StatusCode = HttpStatusCode.BadRequest, Metadata = null, ErrorMessage = e.Message };
            }
        }
    }

}