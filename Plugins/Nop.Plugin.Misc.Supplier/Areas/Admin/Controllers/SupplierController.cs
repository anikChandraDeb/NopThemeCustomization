using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Factories;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Model;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area("admin")]
    public class SupplierController : BasePluginController
    {
        private readonly ISupplierService _supplierService;
        private readonly IPermissionService _permissionService;
        private readonly ISupplierModelFactory _supplierModelFactory;
        protected readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        public SupplierController(
            ISupplierService supplierService,
            IPermissionService permissionService,
            ISupplierModelFactory supplierModelFactory,
            INotificationService notificationService,
            ILocalizationService localizationService
            )
        {
            _supplierService = supplierService;
            _permissionService = permissionService;
            _supplierModelFactory = supplierModelFactory;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        public IActionResult Index()
        {
            var model = _supplierModelFactory.PrepareSupplierSearchModel();

            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Areas/Admin/Views/Supplier/Index.cshtml", model);
        }


        [HttpPost]
        public async Task<IActionResult> List(SupplierSearchModel searchModel)
        {
            var model = await _supplierModelFactory.PrepareSupplierListModelAsync(searchModel);
            return Json(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _supplierModelFactory.PrepareCreateSupplierModelAsync();
            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Areas/Admin/Views/Supplier/Create.cshtml", model);
        }


        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public async Task<IActionResult> Create(SupplierModel model, bool continueEditing)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.Name))
            {
                model.Description = StripPTags(model.Description);

                var supplierEntity = _supplierModelFactory.PrepareEntity(model);

                await _supplierService.InsertAsync(supplierEntity);

                await _supplierModelFactory.SaveLocalizedValuesAsync(supplierEntity, model.Locales);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Supplier.Added"));

                return continueEditing
                    ? RedirectToAction("Edit", new { id = supplierEntity.Id })
                    : RedirectToAction("Index");
            }

            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Areas/Admin/Views/Supplier/Create.cshtml", model);
        }



        public async Task<IActionResult> Edit(int id)
        {
            var supplierEntity = await _supplierService.GetByIdAsync(id);

            if (supplierEntity == null)
                return NotFound();

            var model = await _supplierModelFactory.PrepareEditModelAsync(supplierEntity);

            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Areas/Admin/Views/Supplier/Edit.cshtml", model);
        }


        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Edit(SupplierModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                model.Description = StripPTags(model.Description);

                var supplierEntity = _supplierModelFactory.PrepareEntity(model);
                
                await _supplierService.UpdateAsync(supplierEntity);

                await _supplierModelFactory.SaveLocalizedValuesAsync(supplierEntity, model.Locales);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.Updated"));

                if (!continueEditing)
                    return RedirectToAction("Index");

                return RedirectToAction("Edit", new { id = supplierEntity.Id });
            }

            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Areas/Admin/Views/Supplier/Edit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var supplierEntity = await _supplierService.GetByIdAsync(id);

            if (supplierEntity != null)
                await _supplierService.DeleteAsync(supplierEntity);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AssignSupplierToProduct(int productId, int supplierId)
        {
            if (productId == 0 || supplierId == 0)
                return Json(new { success = false, message = "Invalid product or supplier ID" });

            await _supplierService.InsertOrUpdateProductSupplierMappingAsync(productId, supplierId);

            return Json(new { success = true, message = "Supplier added to product successfully." });
        }

        public static string StripPTags(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remove wrapping <p> and </p> tags only
            return Regex.Replace(input, @"^<p>(.*?)</p>$", "$1", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
    }
}
