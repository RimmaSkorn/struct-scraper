using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

using HtmlAgilityPack;



namespace StructScraper.Models.Metadata
{
    public class HtmlMetadata : ResourceMetadata
    {
        public HtmlMetadata(Uri uri, IEnumerable<string> metaNames) : base(uri, metaNames) { }

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

                    string result = await content.ReadAsStringAsync();
                    if (result != null)
                    {
                        var doc = new HtmlDocument();
                        doc.LoadHtml(result);

                        var list = doc.DocumentNode.SelectNodes("//meta");
                        foreach (var node in list)
                        {
                            string name = node.GetAttributeValue("name", "");
                            if (metaNames.Contains(name, StringComparer.OrdinalIgnoreCase))
                                {
                                string value = node.GetAttributeValue("content", "");
                                if (!String.IsNullOrEmpty(value))
                                {
                                    metadata[name] = value;
                                }
                            }
                            string property = node.GetAttributeValue("property", "");
                            if (metaNames.Contains(property, StringComparer.OrdinalIgnoreCase))
                            {
                                string value = node.GetAttributeValue("content", "");
                                if (!String.IsNullOrEmpty(value))
                                {
                                    metadata[property] = value;
                                }
                            }
                        }

                        var tagnames = (from n in metaNames
                                        where n.StartsWith("<") & n.EndsWith(">")
                                        select n.Substring(1, n.Length - 2));
                        foreach (var tn in tagnames)
                        {
                            HtmlNode node = doc.DocumentNode.SelectSingleNode("//*['" + tn.ToLower() + "' = translate(name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')]");
                            if (node != null)
                            {
                                string value = node.InnerText;
                                if (!String.IsNullOrEmpty(value))
                                {
                                    metadata["<" + tn + ">"] = value;
                                }
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