//viewonwebsite/Initializer.js
define([
    'dojo/_base/declare',
    "epi/_Module",
    "epi-cms/contentediting/PublishMenu",

], function (declare, module, PublishMenu) {
    return declare([module], {
        initialize: function () {

            var originalPostCreate = PublishMenu.prototype.postCreate;
            PublishMenu.prototype.postCreate = function () {
                originalPostCreate.apply(this, arguments);
                this.lastPublishedViewLinkNode.target = "_blank";
            };
        }
    });
});