using EPiServer.Cms.TinyMce.Core;

namespace FoundationCore.Web.Business
{
    public static class ServiceConfigurationContextExtensions
    {
        public static void AddTinyMceConfiguration(this IServiceCollection services)
        {
            var customSettings = new Dictionary<string, object>
            {
                {
                    "extended_valid_elements",
                    "section[*],&[*],i[*],div[*],a[*],span[*],style[*],iframe[*],container[*], h3[*]"
                },
                {
                    "valid_children",
                    "+a[img|h1|h2|h3|h4|h5|h6|p|span|div|i|noscript], +span[a|h1|h2|h3|h4|h5|h6|p|span|i|div|noscript], +div[a|h1|h2|h3|h4|h5|h6|p|span|i|div|noscript]"
                },
                {
                    "style_formats_autohide",
                    true
                }
            };

            services.Configure<TinyMceConfiguration>(config =>
            {
                config.Default()
                    .Menubar("file edit insert view format tools")
                    .DisableMenubar()
                    .AddEpiserverSupport()
                    .AddPlugin(
                        "epi-link epi-personalized-content preview searchreplace autolink directionality visualblocks visualchars fullscreen link media template charmap table pagebreak nonbreaking anchor insertdatetime advlist lists wordcount help anchor code")
                    .Toolbar(
                        "formatselect styles | bold italic | cut copy paste pastetext removeformat | epi-personalized-content | table",
                        "bullist numlist outdent indent | alignleft aligncenter alignright | epi-link unlink anchor | fullscreen | code")
                    .StyleFormats(
                        new { title = "Underlined link", classes = "link link--underlined", selector = "a" },
                        new { title = "Small link", classes = "link link--small", selector = "a" },
                        new { title = "Small underlined link", block = "a", classes = "link link--small--underlined", selector = "a" },
                        new { title = "Medium link", classes = "link link--medium", selector = "a" },
                        new { title = "Medium underlined link", classes = "link link--medium--underlined", selector = "a" },

                        // Headings

                        new
                        {
                            title = "Display",
                            items = new[]
                            {
                                new { title = "Display 01", selector = "p,div", classes = "text--display-1" },
                                new { title = "Display 02", selector = "p,div", classes = "text--display-2" }
                            }
                        },
                        new
                        {
                            title = "Headline",
                            items = new[]
                            {
                                new { title = "Headline 01", block = "h1", classes = "text--headline-1" },
                                new { title = "Headline 02", block = "h2", classes = "text--headline-2" },
                                new { title = "Headline 03", block = "h3", classes = "text--headline-3" },
                                new { title = "Headline 04", block = "h4", classes = "text--headline-4" },
                                new { title = "Headline 05", block = "h5", classes = "text--headline-5" },
                                new { title = "Headline 06", block = "h6", classes = "text--headline-6" }
                            }
                        },
                        new
                        {
                            title = "Paragraph",
                            items = new[]
                            {
                                new { title = "Paragraph Book 01", classes = "text--paragraph-book-1", selector = "p" },
                                new { title = "Paragraph Book 02", classes = "text--paragraph-book-2", selector = "p" },
                                new { title = "Paragraph Book 03", classes = "text--paragraph-book-3", selector = "p" },
                                new { title = "Paragraph Book 04", classes = "text--paragraph-book-4", selector = "p" },
                                new { title = "Paragraph Book 05", classes = "text--paragraph-book-5", selector = "p" }
                            }
                        },
                        new
                        {
                            title = "Paragraph Bold",
                            items = new[]
                            {
                                new { title = "Paragraph Bold 01", classes = "text--paragraph-bold-1", selector = "p" },
                                new { title = "Paragraph Bold 02", classes = "text--paragraph-bold-2", selector = "p" },
                                new { title = "Paragraph Bold 03", classes = "text--paragraph-bold-3", selector = "p" },
                                new { title = "Paragraph Bold 04", classes = "text--paragraph-bold-4", selector = "p" },
                                new { title = "Paragraph Bold 05", classes = "text--paragraph-bold-5", selector = "p" }
                            }
                        }
                    )
                    .RawSettings(customSettings);

            });
        }
    }
}