using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

using iText.Kernel.Pdf;


namespace StructScraper.Models.Metadata
{
    public class PdfMetadata: ResourceMetadata
    {
        public PdfMetadata(Uri uri, IEnumerable<string> metaNames) : base(uri, metaNames) { }

        public override async Task<MetadataResponse> Get()
        {

            try
            {
                IDictionary<string, string> metadata = new Dictionary<string, string>();

                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(uri))
                using (HttpContent content = response.Content)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return new MetadataResponse() { Url = uri.AbsoluteUri, StatusCode = response.StatusCode, Metadata = null, ErrorMessage = response.ReasonPhrase };
                    }

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
                }

                return new MetadataResponse() { Url = uri.AbsoluteUri, StatusCode = HttpStatusCode.OK, Metadata = metadata, ErrorMessage = null };



            }
            catch (Exception e)
            {
                return new MetadataResponse() { Url = uri.AbsoluteUri, StatusCode = HttpStatusCode.BadRequest, Metadata = null, ErrorMessage = e.Message };
            }
        }
    }

}