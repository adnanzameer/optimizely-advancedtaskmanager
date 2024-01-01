using EPiServer.Web.Mvc.Html;
using FoundationCore.Web.Helpers;
using FoundationCore.Web.Models.Interface;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoundationCore.Web.Business.Rendering;

public class MyContentAreaRenderer : ContentAreaRenderer
{
    private string ContentAreaTag { get; set; }


    public override void Render(IHtmlHelper htmlHelper, ContentArea contentArea)
    {
        if (contentArea == null || contentArea.IsEmpty)
        {
            return;
        }

        ContentAreaTag = htmlHelper.ViewData["tag"] as string;

        var viewContext = htmlHelper.ViewContext;
        TagBuilder tagBuilder = null;

        if (!IsInEditMode() && ShouldRenderWrappingElement(htmlHelper))
        {
            tagBuilder = new TagBuilder(GetContentAreaHtmlTag(htmlHelper, contentArea));
            AddNonEmptyCssClass(tagBuilder, viewContext.ViewData["cssclass"] as string);

            viewContext.Writer.Write(tagBuilder.RenderStartTag());
        }

        RenderContentAreaItems(htmlHelper, contentArea.FilteredItems);

        if (tagBuilder == null)
        {
            return;
        }

        viewContext.Writer.Write(tagBuilder.RenderEndTag());
    }

    protected override void RenderContentAreaItem(
        IHtmlHelper htmlHelper,
        ContentAreaItem contentAreaItem,
        string templateTag,
        string htmlTag,
        string cssClass)
    {
        var originalWriter = htmlHelper.ViewContext.Writer;
        var tempWriter = new HtmlStringWriter();
        htmlHelper.ViewContext.Writer = tempWriter;

        try
        {
            var tag = string.IsNullOrEmpty(ContentAreaTag) ? templateTag : ContentAreaTag;

            base.RenderContentAreaItem(htmlHelper, contentAreaItem, tag, htmlTag, cssClass);

            var contentItemContent = tempWriter.ToString();
            var hasEditContainer = htmlHelper.GetFlagValueFromViewData("HasEditContainer");

            // we need to render block if we are in Edit mode
            if (IsInEditMode() && (hasEditContainer == null || hasEditContainer.Value))
            {
                originalWriter.Write(contentItemContent);
                return;
            }

            ProcessItemContent(contentItemContent, htmlHelper, originalWriter);
        }
        finally
        {
            // restore original writer to proceed further with rendering pipeline
            htmlHelper.ViewContext.Writer = originalWriter;
        }
    }

    private void ProcessItemContent(
        string contentItemContent,
        IHtmlHelper htmlHelper,
        TextWriter originalWriter)
    {
        HtmlNode blockContentNode = null;

        var shouldStop = RenderItemContainer(contentItemContent, htmlHelper, originalWriter, ref blockContentNode);
        if (shouldStop)
        {
            return;
        }

        if (blockContentNode == null)
        {
            PrepareNodeElement(ref blockContentNode, contentItemContent);
        }

        if (blockContentNode != null)
        {
            originalWriter.Write(blockContentNode.OuterHtml);
        }
    }

    protected override string GetContentAreaItemCssClass(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
    {
        return GetItemCssClass(htmlHelper, contentAreaItem);
    }

    private string GetItemCssClass(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
    {
        var tag = GetContentAreaItemTemplateTag(htmlHelper, contentAreaItem);
        var baseClasses = base.GetContentAreaItemCssClass(htmlHelper, contentAreaItem);

        return
            $"block {GetTypeSpecificCssClasses(contentAreaItem)}{(!string.IsNullOrEmpty(tag) ? " " + tag : "")}{(!string.IsNullOrEmpty(baseClasses) ? baseClasses : "")}";
    }


    private static string GetTypeSpecificCssClasses(ContentAreaItem contentAreaItem)
    {
        var content = contentAreaItem.LoadContent();
        var cssClass = content?.GetOriginalType().Name.ToLowerInvariant() ?? string.Empty;

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (content is ICustomCssInContentArea customClassContent && !string.IsNullOrWhiteSpace(customClassContent.ContentAreaCssClass))
        {
            cssClass += $" {customClassContent.ContentAreaCssClass}";
        }

        return cssClass;
    }

    private void PrepareNodeElement(ref HtmlNode node, string contentItemContent)
    {
        if (node != null)
        {
            return;
        }

        var doc = new HtmlDocument();
        doc.Load(new StringReader(contentItemContent));
        node = doc.DocumentNode.ChildNodes.FirstOrDefault();
    }

    private bool RenderItemContainer(
        string contentItemContent,
        IHtmlHelper htmlHelper,
        TextWriter originalWriter,
        ref HtmlNode blockContentNode)
    {
        // do we need to control item container visibility?
        var renderItemContainer = htmlHelper.GetFlagValueFromViewData("hasitemcontainer");
        if (renderItemContainer.HasValue && !renderItemContainer.Value)
        {
            PrepareNodeElement(ref blockContentNode, contentItemContent);
            if (blockContentNode != null)
            {
                originalWriter.Write(blockContentNode.InnerHtml);
                return true;
            }
        }

        return false;
    }
}
