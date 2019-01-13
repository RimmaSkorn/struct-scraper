# StructScraper
## Real-time structured data scraper
*StructScraper* is a tool for extracting structured data from web resources in real-time and placing it on web pages.  It includes REST API and jQuery plugins. The REST API extracts data from web resources. The plugins make requests to the REST API, and stamp received data   on the page. 

Currently, StructScraper supports:
* Microdata,  JSON-LD and meta-tag-based markup  in HTML documents
* Embedded and custom properties in Word and PDF documents

The microdata parser grew out of the preliminary implementation of the [Chapleau.MicrodataParser]( https://archive.codeplex.com/?p=microdata) library, with additions and improvements made to resolve bugs and improve code maintainability.

The project uses 
*	[HtmlAgilityPack]( https://www.nuget.org/packages/HtmlAgilityPack/) to parse HTML documents 
*	[iText 7]( https://www.nuget.org/packages/itext7/) to get PDF metadata
*	[Open XML SDK](https://www.microsoft.com/en-us/download/details.aspx?id=30425/)  and 
[DSOfile in 64 bit version]( https://www.codeproject.com/tips/1118708/bit-application-can-not-use-dsofile) to get Word metadata 

