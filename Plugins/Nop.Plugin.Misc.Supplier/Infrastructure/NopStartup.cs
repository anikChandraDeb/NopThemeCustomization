using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.Supplier.Services;

namespace Nop.Plugin.Misc.Supplier.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISupplierService, SupplierService>();
        }

        public void Configure(IApplicationBuilder application) { }

        public int Order => 3000;
    }
}
