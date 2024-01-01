using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc.Html;
using FoundationCore.Web.Business.Rendering;
using FoundationCore.Web.Helpers;
using FoundationCore.Web.Models.Interface;

namespace FoundationCore.Web.Business
{
    [ModuleDependency(typeof(InitializationModule))]
    public class Initialize : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddScoped<IUrlHelpers, UrlHelpers>();

            context.ConfigurationComplete += (_, _) =>
            {
                //Register custom implementations that should be used in favor of the default implementations
                context.Services.AddTransient<ContentAreaRenderer, MyContentAreaRenderer>();
            };
        }

        void IInitializableModule.Initialize(InitializationEngine context)
        {
            // Required 
        }

        void IInitializableModule.Uninitialize(InitializationEngine context)
        {
            // Required 
        }
    }
}
