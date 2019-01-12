using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using System.Net;
using System.Configuration;
using System.IO;

using DSOFile;

namespace StructScraper.Models.Metadata
{
    public class DocMetadata: ResourceMetadata
    {
        public DocMetadata(Uri uri, IEnumerable<string> metaNames) : base(uri, metaNames) { }

        public override async Task<MetadataResponse> Get()
        {
            try
            {
                IDictionary<string, string> metadata = new Dictionary<string, string>();
                string fileName = tmpDir + @"\" + Guid.NewGuid() + ".doc";
                if (!Directory.Exists(tmpDir))
                {
                    Directory.CreateDirectory(tmpDir);
                }


                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(uri, fileName);
                }


                OleDocumentProperties odp = new OleDocumentProperties();
                odp.Open(fileName, true);

                var summaryProps = odp.SummaryProperties;

                /* 
                 * Builtin Document Properties:
                 * 
                       ApplicationName                     Manager
                       Author                              MultimediaClipCount 
                       ByteCount                           NoteCount
                       Category                            PageCount 
                       CharacterCount                      ParagraphCount 
                       CharacterCountWithSpaces            PresentationFormat 
                       Comments                            RevisionNumber 
                       Company                             SharedDocument 
                       DateCreated                         SlideCount 
                       DateLastPrinted                     Subject 
                       DateLastSaved                       Template
                       DigitalSignature                    Thumbnail 
                       DocumentSecurity                    Title 
                       HiddenSlideCount                    TotalEditTime
                       Keywords                            Version
                       LastSavedBy                         WordCount 
                       LineCount 
                */

                /* Only main of them have been implemented */

                if (summaryProps != null)
                {
                    if (metaNames.Contains("Author", StringComparer.OrdinalIgnoreCase))
                    {
                        metadata["Author"] = summaryProps.Author;
                    }
                    if (metaNames.Contains("Category", StringComparer.OrdinalIgnoreCase))
                    {
                        metadata["Category"] = summaryProps.Category;
                    }
                    if (metaNames.Contains("DateCreated", StringComparer.OrdinalIgnoreCase))
                    {
                        metadata["DateCreated"] = ((DateTime)summaryProps.DateCreated).ToString(ISO_DATE_FORMAT);
                    }
                    if (metaNames.Contains("Keywords", StringComparer.OrdinalIgnoreCase))
                    {
                        metadata["Keywords"] = summaryProps.Keywords;
                    }
                    if (metaNames.Contains("DateLastSaved", StringComparer.OrdinalIgnoreCase))
                    {
                        metadata["DateLastSaved"] = ((DateTime)summaryProps.DateLastSaved).ToString(ISO_DATE_FORMAT);
                    }
                    if (metaNames.Contains("LastSavedBy", StringComparer.OrdinalIgnoreCase))
                    {
                        metadata["LastSavedBy"] = summaryProps.LastSavedBy;
                    }
                    if (metaNames.Contains("Subject", StringComparer.OrdinalIgnoreCase))
                    {
                        metadata["Subject"] = summaryProps.Subject;
                    }
                    if (metaNames.Contains("Title", StringComparer.OrdinalIgnoreCase))
                    {
                        metadata["Title"] = summaryProps.Title;
                    }
                    if (metaNames.Contains("Version", StringComparer.OrdinalIgnoreCase))
                    {
                        metadata["Version"] = summaryProps.Version;
                    }

                }

                foreach (CustomProperty p in odp.CustomProperties)
                {
                    if (metaNames.Contains(p.Name,StringComparer.OrdinalIgnoreCase)) {
                        dynamic value = p.get_Value();
                        if (value is DateTime)
                            metadata[p.Name] = ((DateTime)value).ToString(ISO_DATE_FORMAT);
                        else
                            metadata[p.Name] = value.ToString();
                    }
                }

                return new MetadataResponse() { Url = uri.AbsoluteUri, StatusCode = HttpStatusCode.OK, Metadata = metadata, ErrorMessage = null };
            }
            catch (Exception e)
            {
                return new MetadataResponse() { Url = uri.AbsoluteUri, StatusCode = HttpStatusCode.BadRequest, Metadata = null, ErrorMessage = e.Message };
            }

        }

        private const string ISO_DATE_FORMAT = "yyyy-MM-dd";
        static private string tmpDir = ConfigurationManager.AppSettings["TmpDir"];

    }
}