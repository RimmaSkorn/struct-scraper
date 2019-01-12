using System.Threading.Tasks;

namespace StructScraper.Models.JsonLd
{
    public class JsonLdRequest
    {
        public string Url { get; set; }
        public string SchemaType { get; set; }

        public async Task<JsonLdResponse> GetResponse()
        {
            Resource resource = new Resource() { Url = Url };
            return await resource.GetJsonLd(SchemaType);
        }

    }
}