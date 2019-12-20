using System.Threading.Tasks;

namespace StructScraper.Models.Struct
{
    public class StructRequest
    {
        public string Url { get; set; }
        public string SchemaType { get; set; }

        public async Task<StructResponse> GetResponse()
        {
            Resource resource = new Resource() { Url = Url };
            return await resource.GetStruct(SchemaType);
        }

    }
}