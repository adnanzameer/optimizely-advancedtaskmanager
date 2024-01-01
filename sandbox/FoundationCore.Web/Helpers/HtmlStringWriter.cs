using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace FoundationCore.Web.Helpers
{
    public class HtmlStringWriter : StringWriter
    {
        public override void Write(object value)
        {
            if (value is IHtmlContent content)
            {
                content.WriteTo(this, HtmlEncoder.Default);
            }
            else
            {
                base.Write(value);
            }
        }
    }
}
