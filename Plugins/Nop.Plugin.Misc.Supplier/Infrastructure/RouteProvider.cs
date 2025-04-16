using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Supplier.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            // Register your admin controller route
            endpointRouteBuilder.MapControllerRoute(
                name: "Plugin.Misc.Supplier.Admin",
                pattern: "Admin/Supplier/{action=List}/{id?}",
                defaults: new { controller = "Supplier", area = "Admin" });
        }

        public int Priority => 0;
    }
}
