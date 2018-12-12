(function($) {

  $.fn.stampRefMeta = function(options) {

    var apiMultiUri  = options.apiMultiUri;
    var class_metas  = options.class_metas;

    var rsrc_selector     = '.rsrc';
    var rsrc_ref_selector = '.rsrc-ref';

    var pending_class = 'rsrc-pending';
    var done_class    = 'rsrc-done';
    var fail_class    = 'rsrc-fail';


    var getLang = function(elem) {

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

    var formatedDate = function(date, format, lang) {

        var momentDate = new moment(date);
        if (!momentDate.isValid())
            return date;

        if (format)
            return momentDate.format(format);

        momentDate.locale(lang);
        return momentDate.format('L');

    }

    var getMetaInfo = function(metas, name) {
        var meta_objects = $.grep(metas, function (value, index) { return (value.name == name); });
        return meta_objects[0];
    }

    var getMetaNames = function(metas, meta) {
        return getMetaInfo(metas, meta).metas;
    }

     var getRequests = function() {

        var requests = [];

        $(rsrc_ref_selector).each(function (index, elem) {

            var url = elem.href;
            if (!url) return;

            var rsrc_elem = $(elem).closest(rsrc_selector);

            var classes = $(rsrc_elem).find("[class*='rsrc-']").not("[class~='rsrc-ref']")
                .map(function () {
                    return this.classList;
                }).get().join(" ").split(" ");

            var meta_classes = $.grep(classes, function (value, index) { return value.match("^rsrc-") });
            var metas = $.map(meta_classes, function (elem, index) { return elem.substr(5) });
            var meta_names = $.map(metas, function (elem, index) { return getMetaNames(class_metas, elem).join(", ") }).join(", ").split(", ");

            var request = { Url: url, MetaNames: meta_names };

            requests.push(request);

        });

        return requests;

   }

     var stampMetadata = function(data) {

         $(rsrc_ref_selector).each(function (index, elem) {

            var url = elem.href;
            if (!url) return;

            var rsrc_elem = $(elem).closest(rsrc_selector);
            if (!rsrc_elem) return;

            var responses = $.grep(data, function (value, index) {
                return (value.Url == url);
            });
            if (!responses.length) return;

            var response = responses[0];

            if (response.StatusCode == 200) {

                var metadata = {};
                $.each(response.Metadata, function (key, value) { metadata[key.toUpperCase()] = value; return; });

                $(rsrc_elem).find("[class*='rsrc-']").not("[class~='rsrc-ref']")
                    .each(function (index, elem) {
                        var meta_classes = $.grep(elem.classList, function (value, index) { return value.match("^rsrc-") });
                        var meta = meta_classes[0].substr(5);
                        var meta_names = getMetaNames(class_metas, meta);
                        var meta_values = $.map(meta_names, function (elem, index) { return metadata[elem.toUpperCase()] });
                        meta_values = $.grep(meta_values, function (value, index) { return (value); });

                        var meta_value = "";
                        if (meta_values) {
                            meta_value = meta_values[0];
                        }
                        if (meta_value) {
                            var meta_type = getMetaInfo(class_metas, meta).type;
                            if (meta_type == 'date') {
                                if (meta_value.match("^D:")) {
                                    meta_value = meta_value.substr(2,8);
                                }
                                var format = $(elem).data("date-format");
                                var lang = getLang(elem);
                                var frmDate = formatedDate(meta_value, format, lang);
                                meta_value = frmDate;
                            }
                            $(elem).text(meta_value);
                        }
                        else {
                            $(elem).remove();
                        }
                    });

                $(rsrc_elem).addClass(done_class);

            }

            else if (response.StatusCode == 404) {
                $(rsrc_elem).remove();
            }
            else {
                $(rsrc_elem).addClass(fail_class);
            };
        });
    }

    var loadMetadata = function() {

        $(rsrc_selector).addClass(pending_class);
        var requests = getRequests();

        $.support.cors = true;
        $.ajax({
        url: apiMultiUri,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(requests)
        })
        .done(function (data) {
            stampMetadata(data);
        })
        .fail(function (jqXHR, textStatus, err) {
            alert(textStatus + ', ' + err);
        })
        .always(function () {
            $(rsrc_selector).removeClass(pending_class);
        })
        ;
    }

    loadMetadata();

  }; 
})(jQuery);

 