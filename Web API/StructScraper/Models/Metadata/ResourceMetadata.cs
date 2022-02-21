using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;

namespace StructScraper.Models.Metadata
{
    public class ResourceMetadata
    {

 
        public ResourceMetadata(Resource resource, IEnumerable<string> metaNames) // ?? may be not needed
        {
            this.resource = resource;
            this.metaNames = metaNames;
        }

        protected IEnumerable<string> metaNames;

        protected Resource resource; 

        public async Task<MetadataResponse> Get()
        {
            try
            {
                using (HttpResponseMessage response = await resource.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return new MetadataResponse() { Url = resource.Url, StatusCode = response.StatusCode, Metadata = null, ErrorMessage = response.ReasonPhrase };
                    }

                    ResourceMetadata metadata;
                    using (HttpContent content = response.Content)
                    {
                        ResourceType resourceType = Resource.GetType(content);
                        switch (resourceType)
                        {
                            case ResourceType.Html:
                                metadata = new HtmlMetadata(resource, metaNames);
                                break;
                            case ResourceType.Pdf:
                                metadata = new PdfMetadata(resource, metaNames);
                                break;
                            case ResourceType.Docx:
                                metadata = new DocxMetadata(resource, metaNames);
                                break;
                            case ResourceType.Doc:
                                metadata = new DocMetadata(resource, metaNames);
                                break;
                            case ResourceType.Unsupported:
                                return new MetadataResponse() { Url = resource.Url, StatusCode = HttpStatusCode.UnsupportedMediaType, Metadata = null, ErrorMessage = "Unsupported MIME type" };
                            default:
                                metadata = new HtmlMetadata(resource, metaNames);
                                break;
                        }
                        return await metadata.Get(content);

                    }
                }
            }
            catch (Exception e)
            {
                return new MetadataResponse() { Url = resource.Url, StatusCode = HttpStatusCode.BadRequest, Metadata = null, ErrorMessage = e.Message };

            }
        }

        public async virtual Task<MetadataResponse> Get(HttpContent content)
        {
            return new MetadataResponse();
        }

    }
}