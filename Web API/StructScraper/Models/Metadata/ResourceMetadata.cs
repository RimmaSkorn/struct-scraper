using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace StructScraper.Models.Metadata
{
    public class ResourceMetadata
    {

        protected ResourceMetadata(Uri uri, IEnumerable<string> metaNames)
        {
            this.uri = uri;
            this.metaNames = metaNames;
        }

        protected Uri uri;
        protected IEnumerable<string> metaNames;

        public async virtual Task<MetadataResponse> Get()
        {
            return new MetadataResponse();
        }



    }
}