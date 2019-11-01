using EPiServer.Forms.Core;
using EPiServer.Forms.Demo.Implementation;
using EPiServer.Forms.Demo.Security;
using EPiServer.Forms.Internal.Security;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Hosting;

namespace EPiServer.Forms.Demo
{
    public class DemoInitializationModule : IInitializableModule, IConfigurableModule
    {

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.ConfigurationComplete += (o, e) =>
            {
                context.Services.AddTransient<IAppendExtraInfoToRedirection, AppendInfoToRedirection>()
                    .AddTransient<IAntiForgeryService, CustomAntiForgeryService>();
            };
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters) {}
        public void Uninitialize(InitializationEngine context) {}
    }
}
