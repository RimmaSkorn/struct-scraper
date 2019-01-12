using System.Collections.Generic;
using System.Threading.Tasks;

namespace StructScraper.Models.Metadata
{
    public class MetadataRequest
    {
        public string Url { get; set; }
        public IEnumerable<string> MetaNames;

        public async Task<MetadataResponse> GetResponse()
        {
            Resource resource = new Resource() { Url = Url };
            return await resource.GetMetadata(MetaNames);
        }
    }
}