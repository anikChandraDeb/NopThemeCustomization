using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Factories;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Services;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ISupplierModelFactory, SupplierModelFactory>();
            // Register view location expander
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new PluginViewLocationExpander());
            });
        }

        public void Configure(IApplicationBuilder application) { }

        public int Order => 3000;
    }
}
