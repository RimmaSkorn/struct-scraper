# StructScraper
## Real-time structured data scraper
*StructScraper* is a tool for extracting structured data from web resources in real-time and placing it on web pages.  It includes REST API and jQuery plugins. The REST API extracts data from web resources. The plugins make requests to the REST API and stamp received data   on the page. 

Currently, StructScraper supports:
* Microdata,  JSON-LD and meta-tag-based markup  in HTML documents
* Embedded and custom properties in Word and PDF documents

The microdata parser grew out of the preliminary implementation of the [Chapleau.MicrodataParser]( https://archive.codeplex.com/?p=microdata) library, with additions and improvements made to resolve bugs and improve code maintainability.
