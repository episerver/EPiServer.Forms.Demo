using EPiServer.Forms.Core;
using EPiServer.Forms.Demo.Implementation;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Hosting;

namespace EPiServer.Forms.Demo
{
    public class DemoInitializationModule : IInitializableModule, IConfigurableModule
    {
        protected ServiceConfigurationContext _serviceConfigurationContext;

        public void ConfigureContainer(ServiceConfigurationContext serviceConfigurationContext)
        {
            _serviceConfigurationContext = serviceConfigurationContext;
        }

        public void Initialize(InitializationEngine context)
        {
            _serviceConfigurationContext.Container.Configure(c =>
            {
                c.For<IAppendExtraInfoToRedirection>().Use(new AppendInfoToRedirection());  // use our demo extra info
            });
        }

        public void Preload(string[] parameters) {}
        public void Uninitialize(InitializationEngine context) {}
        
    }
}
