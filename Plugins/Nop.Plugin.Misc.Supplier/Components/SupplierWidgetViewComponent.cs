using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Supplier.Services;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.Supplier.Components;
public class SupplierWidgetViewComponent : NopViewComponent
{
    private readonly ISupplierService _supplierService; // Inject the SupplierService

    public SupplierWidgetViewComponent(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        // Check if the widget should be shown in the product page
        if (widgetZone == AdminWidgetZones.ProductDetailsBlock)
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Default.cshtml", suppliers);
        }

        // Return an empty content result for other widget zones
        return Content(string.Empty);
    }
}
