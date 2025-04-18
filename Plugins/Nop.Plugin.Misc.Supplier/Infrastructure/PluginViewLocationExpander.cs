using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Plugin.Misc.Supplier.Infrastructure;
public class PluginViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
        // No values to add
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        var pluginViewLocations = new[]
        {
            "/Plugins/Nop.Plugin.Misc.Supplier/Views/{1}/{0}.cshtml",
            "/Plugins/Nop.Plugin.Misc.Supplier/Views/Shared/{0}.cshtml"
        };

        return pluginViewLocations.Concat(viewLocations);
    }
}

