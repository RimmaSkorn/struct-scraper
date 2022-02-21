using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.CustomProperties;


namespace StructScraper.Models.Metadata
{
    public class DocxMetadata : ResourceMetadata
    {
        public DocxMetadata(Resource resource, IEnumerable<string> metaNames) : base(resource, metaNames) { }

        public override async Task<MetadataResponse> Get(HttpContent content)
        {
            try
            {
                IDictionary<string, string> metadata = new Dictionary<string, string>();

                using (var memStream = new MemoryStream())
                {
                    await content.CopyToAsync(memStream);

                    using (var document = WordprocessingDocument.Open(memStream, false))
                    {
                        var packageProps = document.PackageProperties;

                        /*
                         * Document properties:
                         * 
                         Category          Language
                         ContentStatus     LastModifiedBy 
                         ContentType       Modified
                         Created           Revision
                         Creator           Subject
                         Description       Title
                         Identifier        Version
                         Keywords
                         */

                        if (packageProps != null)
                        {
                            if (metaNames.Contains("Category", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["Category"] = packageProps.Category;
                            }
                            if (metaNames.Contains("ContentStatus", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["ContentStatus"] = packageProps.ContentStatus;
                            }
                            if (metaNames.Contains("ContentType", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["ContentType"] = packageProps.ContentType;
                            }
                            if (metaNames.Contains("Created", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["Created"] = packageProps.Created.ToString();
                            }
                            if (metaNames.Contains("Creator", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["Creator"] = packageProps.Creator;
                            }
                            if (metaNames.Contains("Description", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["Description"] = packageProps.Description;
                            }
                            if (metaNames.Contains("Keywords", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["Keywords"] = packageProps.Keywords;
                            }
                            if (metaNames.Contains("Language", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["Language"] = packageProps.Language;
                            }
                            if (metaNames.Contains("LastModifiedBy", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["LastModifiedBy"] = packageProps.LastModifiedBy;
                            }
                            if (metaNames.Contains("Modified", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["Modified"] = packageProps.Modified.ToString();
                            }
                            if (metaNames.Contains("Revision", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["Revision"] = packageProps.Revision;
                            }
                            if (metaNames.Contains("Subject", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["Subject"] = packageProps.Subject;
                            }
                            if (metaNames.Contains("Title", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["Title"] = packageProps.Title;
                            }
                            if (metaNames.Contains("Version", StringComparer.OrdinalIgnoreCase))
                            {
                                metadata["Version"] = packageProps.Version;
                            }
                        }
                        var customProps = document.CustomFilePropertiesPart;
                        if (customProps != null)
                        {
                            var props = customProps.Properties;
                            if (props != null)
                            {
                                foreach (var p in props)
                                {
                                    CustomDocumentProperty customProp = (CustomDocumentProperty)p;

                                    if (metaNames.Contains(customProp.Name.Value, StringComparer.OrdinalIgnoreCase))
                                    {
                                        metadata[customProp.Name.Value] = customProp.InnerText;
                                    }

                                }
                            }
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