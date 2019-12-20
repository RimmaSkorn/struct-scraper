using System.Net;

namespace StructScraper.Models
{
    public abstract class Response
    {
        public string Url { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}