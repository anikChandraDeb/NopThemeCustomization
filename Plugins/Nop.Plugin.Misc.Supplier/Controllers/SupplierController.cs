using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Supplier.Domain;
using Nop.Plugin.Misc.Supplier.Services;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Supplier.Controllers
{
    //[AuthorizeAdmin]
    //[Area("admin")]

    public class SupplierController : BasePluginController
    {
        private readonly ISupplierService _supplierService;
        private readonly IPermissionService _permissionService;

        public SupplierController(ISupplierService supplierService, IPermissionService permissionService)
        {
            _supplierService = supplierService;
            _permissionService = permissionService;
        }

        public async Task<IActionResult> Index()
        {
            

            var suppliers = await _supplierService.GetAllAsync();
            return View("~/Plugins/Misc.Supplier/Views/Supplier/Index.cshtml", suppliers);
        }

        public IActionResult Create()
        {
            return View("~/Plugins/Misc.Supplier/Views/Supplier/Create.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Create(SupplierEntity supplier)
        {
            if (ModelState.IsValid)
            {
                await _supplierService.InsertAsync(supplier);
                return RedirectToAction("Index");
            }
            return View("~/Plugins/Misc.Supplier/Views/Supplier/Create.cshtml", supplier);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            return View("~/Plugins/Misc.Supplier/Views/Supplier/Edit.cshtml", supplier);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SupplierEntity supplier)
        {
            if (ModelState.IsValid)
            {
                await _supplierService.UpdateAsync(supplier);
                return RedirectToAction("Index");
            }
            return View("~/Plugins/Misc.Supplier/Views/Supplier/Edit.cshtml", supplier);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier != null)
                await _supplierService.DeleteAsync(supplier);

            return RedirectToAction("Index");
        }
    }
}
