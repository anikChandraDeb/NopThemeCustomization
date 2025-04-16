using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Supplier.Domain;
using Nop.Plugin.Misc.Supplier.Model;
using Nop.Plugin.Misc.Supplier.Services;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;
using Nop.Services.Common;
using Nop.Core;

namespace Nop.Plugin.Misc.Supplier.Controllers
{
    [AuthorizeAdmin]
    [Area("admin")]

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
            var model = new SupplierSearchModel();
            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Index.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> List(SupplierSearchModel searchModel)
        {
            // Get paginated suppliers from service
            var suppliers = await _supplierService.GetAllAsync(
                searchModel.SearchName,
                searchModel.SearchEmail,
                searchModel.Page - 1,
                searchModel.PageSize
            );

            // Convert domain models to view models
            var supplierModels = suppliers.Select(s => new SupplierModel
            {
                Id = s.Id,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address
            }).ToList();

            // Prepare the result in the format Nop's DataTables expects
            var model = new SupplierListModel
            {
                Data = supplierModels
            };

            return Json(model);
        }






        public IActionResult Create()
        {
            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Create.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Create(SupplierEntity supplier)
        {
            if (ModelState.IsValid)
            {
                await _supplierService.InsertAsync(supplier);
                return RedirectToAction("Index");
            }
            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Create.cshtml", supplier);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Edit.cshtml", supplier);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SupplierEntity supplier)
        {
            if (ModelState.IsValid)
            {
                await _supplierService.UpdateAsync(supplier);
                return RedirectToAction("Index");
            }
            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Edit.cshtml", supplier);
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
