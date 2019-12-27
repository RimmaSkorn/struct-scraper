(function ($) {
    $.fn.fillRefStruct = function (options) {

        var apiMultiUri = options.apiMultiUri;
        var import_schema = options.import_schema;

        var import_selector = '.import-struct';
        var import_url_selector = '.struct-url';

        var pending_class = 'import-pending';
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

        var getRequests = function() {

            var requests = [];

            $(import_url_selector).each(function (index, elem) {

                var url = elem.href;
                if (!url) return;

                var request = { Url: url, SchemaType: import_schema };
                requests.push(request);

            });

            return requests;
        }

        var SetString = function (elem, value) {
            if (Date.parse(value) && (!$.isNumeric(value)) && (!/[а-яА-ЯЁё]/.test(value))) {
                var lang = getLang(elem);
                value = formatedDate(value, NaN, lang);
            };
            if (elem.is("img"))
                elem.attr("src", value);
            else
                elem.text(value);
        }

        var SetPropString = function (parentElem, key, value) {

            var propElem = $(parentElem).find("[itemprop=" + key + "]").first();
            SetString(propElem, value);
        }

        var SetObject = function (elem, object, context) {

            elem.attr("itemscope", "");

            if (object.itemType)
                elem.attr("itemtype", object.itemType);
            else if (context && object["@type"])
                elem.attr("itemtype", context + '/' + object["@type"]);

            $.each(object, function (key, value) {
                if ((key != "itemType") && (key != "@type") && (key != "itemId") && (key != "@id")) {
                    if (typeof value == 'string') {
                        SetPropString(elem, key, value);
                    } else if (Array.isArray(value)) {
                        SetPropArray(elem, key, value, context);
                    } else if (typeof value == 'object') {
                        SetPropObject(elem, key, value, context);
                    }
                }
            });
        }

        var SetPropObject = function (parentElem, key, object, context) {

            var propElem = $(parentElem).find("[itemprop=" + key + "]").first();
            SetObject(propElem, object, context);
        }

        var SetPropArray = function (parentElem, key, arr, context) {

            var propElem = $(parentElem).find("[itemprop=" + key + "]").first();
            if (!propElem.attr("data-unique")) {
                var i;
                for (i = 1; i < arr.length; i++) {
                    var copyElem = $(propElem).clone(true);
                    if (typeof arr[i] == 'string') {
                        SetString(copyElem, arr[i]);
                    } else if (typeof arr[i] == 'object') {
                        SetObject(copyElem, arr[i], context);
                    }
                    copyElem.appendTo($(propElem).parent());
                }

            }

            if (typeof arr[0] == 'string') {
                SetString(propElem, arr[0]);
            }
            else if (typeof arr[0] == 'object') {
                SetObject(propElem, arr[0], context);
            };
        }

        var RemoveEmptyProps = function (elem) {
            var props = $(elem).find("[itemprop]");
            props.filter('img[src=""]').remove();
            props.filter("img:not([src])").remove();
            props.filter(":not(img):empty").remove();
            props.not("img").filter(function () { return $.trim($(this).text()) == ""; }).remove();
        }

        var stampStructItem = function(import_elem, struct) {

            $(import_elem).attr("itemscope", "");
            var context = struct["@context"];

            if (struct.itemType)
                $(import_elem).attr("itemtype", struct.itemType);
            else if (struct["@context"] && struct["@type"])
                $(import_elem).attr("itemtype", context + '/' + struct["@type"]);

            $.each(struct, function (key, value) {
                if ((key != "itemType") && (key != "@context") && (key != "@type") && (key != "itemId") && (key != "@id") ) {
                    if (typeof value == 'string') {
                        SetPropString(import_elem, key, value);
                    } else if (Array.isArray(value)) {
                        SetPropArray(import_elem, key, value, context);
                    } else if (typeof value == 'object') {
                        SetPropObject(import_elem, key, value, context);
                    }
                }
            });
            RemoveEmptyProps(import_elem);
        }

        var stampStruct = function(data) {

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
                    stampStructItem(import_elem, response.Struct);
                } else if (response.StatusCode == 404) {
                    $(import_elem).remove();
                } else {
                    $(elem).text(response.ErrorMessage);
                    $(import_elem).addClass(fail_class);
                }
                ;
            });
        }

        var loadStruct = function() {

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
                    stampStruct(data);
                })
                .fail(function (jqXHR, textStatus, err) {
                    alert(textStatus + ', ' + err);
                })
                .always(function () {
                    $(import_selector).removeClass(pending_class);
                })
                ;

        }

        loadStruct();
    };
})(jQuery);



