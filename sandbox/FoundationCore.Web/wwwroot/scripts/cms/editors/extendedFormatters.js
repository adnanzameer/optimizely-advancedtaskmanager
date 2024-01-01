define([
    // dojo
    "dojo/_base/lang",
    "dojo/Deferred",
    // epi
    "epi/dependency",
    "epi-cms/core/PermanentLinkHelper"
],
    function (
        // dojo
        lang,
        Deferred,
        // epi
        dependency,
        PermanentLinkHelper
    ) {
        function getContentByContentLink(contentLink, callback) {
            if (!contentLink) {
                return '';
            }

            var registry = dependency.resolve("epi.storeregistry");
            var store = registry.get("epi.cms.content.light");

            var contentData;

            dojo.when(store.get(contentLink), function (returnValue) {
                contentData = returnValue;
                callback(contentData);
            });

            return contentData;
        }

      var extendedFormatters = {

            urls: {},

            urlFormatter: function (value) {
                if (!value) {
                    return '';
                }

              if (!extendedFormatters.urls[value]) {
                    return value;
                }

              return extendedFormatters.urls[value];
            },

            getContentUrlByPermanentLink: function (link) {

                if (!link) {
                    return '';
                }

                var def = new Deferred();

              if (extendedFormatters.urls[link]) {
                    def.resolve();
                    return def.promise;
                }

                dojo.when(PermanentLinkHelper.getContent(link), function (contentData) {
                    if (contentData) {
                      extendedFormatters.urls[link] = contentData.publicUrl;
                    } else {
                        // Probably an external link.
                      extendedFormatters.urls[link] = link;
                    }
                    def.resolve();
                });

                return def.promise;
            },

            getContentUrlByContentLink: function (contentLink) {

                if (!contentLink) {
                    return '';
                }

                var def = new Deferred();

              if (typeof extendedFormatters.urls === 'undefined') {
                extendedFormatters.urls = {};
              }

              if (extendedFormatters.urls[contentLink]) {
                    def.resolve();
                    return def.promise;
                }

                getContentByContentLink(contentLink, function (contentData) {
                    if (contentData) {
                      extendedFormatters.urls[contentLink] = contentData.publicUrl;
                    }
                    def.resolve();
                });

                return def.promise;
            },

            htmlFormatter: function (value) {
                if (!value)
                    return '';

                return value;
            },
            linkItemsFormatter: function (value) {
                if (!value)
                    return '';

                var toReturn = '';
                for (var i = 0; i < value.length; i++) {
                    toReturn = toReturn + '<u>' + value[i].text + "</u>";

                    if (i !== value.length - 1) {
                        toReturn = toReturn + '<br/>';
                    }
                }
                return toReturn;
            },
            setUrlMappings: function (urlMappings) {
              extendedFormatters.urls = urlMappings;
            }
        };

        return extendedFormatters;
    });
