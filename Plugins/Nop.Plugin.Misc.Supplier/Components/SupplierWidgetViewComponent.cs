using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Supplier.Model;
using Nop.Plugin.Misc.Supplier.Services;
using Nop.Web.Areas.Admin.Models.Catalog;
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
        var productModel = additionalData as ProductModel;

        // Create page: Id == 0 or productModel is null
        if (productModel == null || productModel.Id == 0)
        {
            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Components/Create.cshtml");
        }

        // Edit page: Id > 0
        var suppliers = await _supplierService.GetAllSuppliersAsync();

        var model = new SupplierProductModel
        {
            ProductId = productModel.Id,
            Suppliers = suppliers
        };

        return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Components/Edit.cshtml", model);

        // Return an empty content result for other widget zones
        return Content(string.Empty);
    }
}
