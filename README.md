# StructScraper
## Real-time structured data scraper
*StructScraper* is a tool for extracting structured data from web resources in real-time and placing it on web pages.  It includes REST API on .NET Framework and jQuery plugins. The REST API extracts data from web resources. The plugins make requests to the REST API, and stamp received data   on the page. 

Currently, StructScraper supports:
* Microdata,  JSON-LD and meta-tag-based markup  in HTML documents
* Embedded and custom properties in Word and PDF documents

The microdata parser grew out of the preliminary implementation of the [Chapleau.MicrodataParser]( https://archive.codeplex.com/?p=microdata) library, with additions and improvements made to resolve bugs and improve code maintainability.

The project uses 
*	[HtmlAgilityPack]( https://www.nuget.org/packages/HtmlAgilityPack/) to parse HTML documents 
*	[iText 7]( https://www.nuget.org/packages/itext7/) to get PDF metadata
*	[Open XML SDK](https://www.microsoft.com/en-us/download/details.aspx?id=30425/)  and 
[DSOfile in 64 bit version]( https://www.codeproject.com/tips/1118708/bit-application-can-not-use-dsofile) to get Word metadata 

## Usage

To incorporate semantic data from external resources into your HTML page while the page is loading, add special markup and include a piece of javascript code, as in the following example.

```html
…
<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"> </script>
<script src="https://struct-scraper.keldysh.ru/Scripts/fill-struct.js"> </script>
<script type="text/javascript">
    $(document).ready(function () {
        $(document).fillRefStruct({
            apiMultiUri: 'https://struct-scraper.keldysh.ru/api/struct/multi-uri',
            import_schema: "LocalBusiness, Store, Organization"
        });
    });
</script>
…
<li class="import-struct">
    <div>
        <a class="struct-url" href="https://www.thesparrowsgr.com/" itemprop="name">The Sparrows Coffee & Tea & Newsstand</a>
        <div itemprop="telephone"></div>
        <div itemprop="email"></div>
        <div itemprop="address">
            <span itemprop="postalCode"></span>
            <span itemprop="addressLocality"></span>
            <span itemprop="streetAddress"></span>
        </div>
    </div>
</li>
…
```

Use `.import-struct` class for external data container and `.struct-url` class for hyperlink to external resource. Use `[itemprop]` attribute for [Schema.org]( http://schema.org/) property to be included. jQuery plugin `fillRefStruct` embedded in `fill-struct.js` file performs the job of filling external data.

