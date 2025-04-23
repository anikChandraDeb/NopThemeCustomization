using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                name: "Plugin.Misc.Supplier.Admin",
                pattern: "Admin/Supplier/{action=List}/{id?}",
                defaults: new { controller = "Supplier", area = "Admin" });
        }

        public int Priority => 0;
    }
}
