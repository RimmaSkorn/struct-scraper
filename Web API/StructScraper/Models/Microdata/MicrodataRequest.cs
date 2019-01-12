using System.Threading.Tasks;

namespace StructScraper.Models.Microdata
{
    public class MicrodataRequest
    {
        public string Url { get; set; }
        public string SchemaType { get; set; }

        public async Task<MicrodataResponse> GetResponse()
        {
            Resource resource = new Resource() { Url = Url };
            return await resource.GetMicrodata(SchemaType);
        }

    }
}