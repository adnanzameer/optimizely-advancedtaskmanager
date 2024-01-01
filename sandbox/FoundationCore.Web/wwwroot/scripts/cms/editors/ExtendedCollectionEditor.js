//"foundationcore/editors/extendedFormatters"

define([
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/DeferredList",
    "epi-cms/contentediting/editors/CollectionEditor",
    "foundationcore/editors/extendedFormatters"
],
    function (
        array,
        declare,
        lang,
        DeferredList,
        CollectionEditor,
        extendedFormatters
    ) {
        return declare([CollectionEditor], {
            _getGridDefinition: function () {
                var result = this.inherited(arguments);

                if (this.hasOwnProperty('fieldNames') && this.fieldNames) {
                    extendedFormatters.setUrlMappings(this.urlMappings);

                    for (var i = 0; i < this.fieldNames.length; i++) {
                        result[this.fieldNames[i]].formatter = extendedFormatters.urlFormatter;
                    }
                }

                if (this.hasOwnProperty('mappedHtmlProperties') && this.mappedHtmlProperties) {

                    for (let j = 0; j < this.mappedHtmlProperties.length; j++) {
                        result[this.mappedHtmlProperties[j]].formatter = extendedFormatters.htmlFormatter;
                    }
                }

                if (this.hasOwnProperty('mappedLinkItems') && this.mappedLinkItems) {

                    for (let k = 0; k < this.mappedLinkItems.length; k++) {
                        result[this.mappedLinkItems[k]].formatter = extendedFormatters.linkItemsFormatter;
                    }
                }

                return result;
            },
            onExecuteDialog: function () {

                if (this.hasOwnProperty('fieldNames') && this.fieldNames) {

                    var item = this._itemEditor.get("value");

                    if (item) {
                        var contentUrls = [];

                        for (var i = 0; i < this.fieldNames.length; i++) {
                            var value = item[this.fieldNames[i]];

                            if (value) {
                                if (isNaN(value)) {
                                    contentUrls.push(extendedFormatters.getContentUrlByPermanentLink(value));
                                } else {
                                    contentUrls.push(extendedFormatters.getContentUrlByContentLink(value));
                                }
                            }
                        }

                        var dl = new DeferredList(contentUrls);

                        dl.then(lang.hitch(this,
                            function () {
                                if (this._editingItemIndex !== undefined) {
                                    this.model.saveItem(item, this._editingItemIndex);
                                } else {
                                    this.model.addItem(item);
                                }
                            }));
                    }
                } else {
                    this.inherited(arguments);
                }
            }
        });
    });
