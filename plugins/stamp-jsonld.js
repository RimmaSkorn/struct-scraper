(function ($) {
    $.fn.stampRefJsonld = function (options) {

        var apiSingleUri = options.apiSingleUri;
        var apiMultiUri = options.apiMultiUri;
        var import_schema = options.import_schema;

        var import_selector = '.import-jsonld';
        var import_url_selector = '.jsonld-url';

        var pending_class = 'import-pending';
        var done_class = 'rsrc-done';
        var fail_class = 'rsrc-fail';

        var getLang = function (elem) {

            var elemLang = $(elem).closest("[lang]").attr("lang");
            if (elemLang) {
                return elemLang;
            }

            var navigatorLang = navigator.language;
            if (!navigatorLang) {
                navigatorLang = navigator.browserLanguage;
            };
            return navigatorLang;
        }

        var formatedDate = function (date, format, lang) {
            var momentDate = new moment(date);
            if (!momentDate.isValid())
                return date;
            if (format)
                return momentDate.format(format);
            momentDate.locale(lang);
            return momentDate.format('L');
        }

        var getRequests = function () {

            var requests = [];

            $(import_url_selector).each(function (index, elem) {

                var url = elem.href;
                if (!url) return;

                var request = { Url: url, SchemaType: import_schema };
                requests.push(request);

            });

            return requests;
        }

        var AppendPropString = function(elem, key, value) {
            if (Date.parse(value) && (!/[а-яА-ЯЁё]/.test(value)) && (!$.isNumeric(value))) {
                var lang = getLang(elem);
                value = formatedDate(value, NaN, lang);
            };
            $('<span>').appendTo($(elem)).attr("itemprop", key).text(value);
        }

        var AppendPropObject = function(elem, key, object, context) {
            var newElem = $('<span>').appendTo($(elem)).attr("itemprop", key).attr("itemscope", "");
            if (context && object["@type"])
                newElem.attr("itemtype", context + '/' + object["@type"]);
            $.each(object, function (key, value) {
                if (key != "@type") {
                    if (typeof value == 'string') {
                        AppendPropString(newElem, key, value);
                    } else if (Array.isArray(value)) {
                        AppendPropArray(newElem, key, value);
                    } else if (typeof value == 'object') {
                        AppendPropObject(newElem, key, value);
                    }
                }
            });
        }

        var AppendPropArray = function(elem, key, arr) {
            if (typeof arr[0] == 'string') {
                $('<span>').appendTo($(elem)).attr("itemprop", key).text(arr);
            }
            else if (typeof arr[0] == 'object') {
                $.each(arr, function (index, value) {
                    AppendPropObject(elem, key, value);
                });
            }
        }

        var stampJsonldItem = function(import_elem, jsonlddata) {

            $(import_elem).attr("itemscope", "");
            var context = jsonlddata["@context"];
            if (jsonlddata["@context"] && jsonlddata["@type"])
                $(import_elem).attr("itemtype", context + '/' + jsonlddata["@type"]);

            $.each(jsonlddata, function (key, value) {
                if ((key != "@context") && (key != "@type") && (key != "name")) {
                    if (typeof value == 'string') {
                        AppendPropString(import_elem, key, value);
                    } else if (Array.isArray(value)) {
                        AppendPropArray(import_elem, key, value);
                    }
                    else if (typeof value == 'object') {
                        AppendPropObject(import_elem, key, value, context);
                    }
                }
            });

        }

        var stampJsonld = function(data) {

            $(import_url_selector).each(function (index, elem) {

                var url = elem.href;
                if (!url) return;

                var import_elem = $(elem).closest(import_selector);
                if (!import_elem) return;

                var responses = $.grep(data, function (value, index) {
                    return (value.Url == url);
                });
                if (!responses.length) return;
                var response = responses[0];

                if (response.StatusCode == 200) {

                    stampJsonldItem(import_elem, response.JsonLd);

                    if (response.JsonLd.name) {
                        $(elem).attr("itemprop", "name");
                        if (Array.isArray(response.JsonLd.name))
                            $(elem).text(response.JsonLd.name[0]);
                        else
                            $(elem).text(response.JsonLd.name);
                    }
                }
                else if (response.StatusCode == 404) {
                    $(import_elem).remove();
                }
                else {
                    $(elem).text(response.ErrorMessage);
                    $(import_elem).addClass(fail_class);

                }
                ;


            });
        }

        var loadJsonLd = function() {

            $(import_selector).addClass(pending_class);

            var requests = getRequests();

            $.support.cors = true;
            $.ajax({
                url: apiMultiUri,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(requests)
            })
                .done(function (data) {
                    stampJsonld(data);
                })
                .fail(function (jqXHR, textStatus, err) {
                    alert(textStatus + ', ' + err);
                })
                .always(function () {
                    $(import_selector).removeClass(pending_class);
                })
                ;

        }

        loadJsonLd();

};
})(jQuery);

