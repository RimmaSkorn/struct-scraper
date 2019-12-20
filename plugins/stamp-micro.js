(function ($) {
    $.fn.stampRefMicro = function (options) {

        var apiMultiUri = options.apiMultiUri;
        var import_schema = options.import_schema;

        var import_selector = '.import-micro';
        var import_url_selector = '.micro-url';

        var pending_class = 'import-pending';
        var fail_class = 'rsrc-fail';

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

        var AppendPropString = function(elem, key, value) {
            $('<span>').appendTo($(elem)).attr("itemprop", key).text(value);
        }

        var AppendPropObject = function(elem, key, object) {
            var newElem = $('<span>').appendTo($(elem)).attr("itemprop", key).attr("itemscope", "");
            if (object.itemType)
                newElem.attr("itemtype", object.itemType);
            $.each(object, function (key, value) {
                if ((key != "itemType") && (key != "itemId")) {
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

        var stampMicroItem = function(import_elem, microdata) {

            $(import_elem).attr("itemscope", "");
            if (microdata.itemType)
                $(import_elem).attr("itemtype", microdata.itemType);

            $.each(microdata, function (key, value) {
                if ((key != "itemType") && (key != "itemId") && (key != "name")) {
                    if (typeof value == 'string') {
                        AppendPropString(import_elem, key, value);
                    } else if (Array.isArray(value)) {
                        AppendPropArray(import_elem, key, value);
                    }
                    else if (typeof value == 'object') {
                        AppendPropObject(import_elem, key, value);
                    }
                }
            });

        }

        var stampMicro = function(data) {

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

                    stampMicroItem(import_elem, response.Microdata);

                    if (response.Microdata.name) {
                        $(elem).attr("itemprop", "name");
                        if (Array.isArray(response.Microdata.name))
                            $(elem).text(response.Microdata.name[0]);
                        else
                            $(elem).text(response.Microdata.name);
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

        var loadMicro = function() {

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
                    stampMicro(data);
                })
                .fail(function (jqXHR, textStatus, err) {
                    alert(textStatus + ', ' + err);
                })
                .always(function () {
                    $(import_selector).removeClass(pending_class);
                })
                ;

        }

        loadMicro();
    };
})(jQuery);



