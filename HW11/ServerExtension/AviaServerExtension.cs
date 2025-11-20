using AviaServerExtension.ASE;
using DocsVision.WebClient.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace AviaServerExtension
{
    public class AviaServerExtension : WebClientExtension
    {
        public override string ExtensionName => "AviaServerExtension";
        public override Version ExtensionVersion => new Version(1, 0, 0);

        public override void InitializeServiceCollection(IServiceCollection services)
        {
            base.InitializeServiceCollection(services);

            services.AddHttpClient<IASEService, ASEServiceImpl>();
        }
    }
}