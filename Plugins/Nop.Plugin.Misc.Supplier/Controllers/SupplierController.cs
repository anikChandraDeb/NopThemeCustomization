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
using Nop.Plugin.Misc.Supplier.Factories;
using Nop.Core.Domain.Vendors;
using Nop.Services.Messages;
using Nop.Services.Localization;

namespace Nop.Plugin.Misc.Supplier.Controllers
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
            ILocalizationService localizationService)
        {
            _supplierService = supplierService;
            _permissionService = permissionService;
            _supplierModelFactory = supplierModelFactory;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }



        public async Task<IActionResult> Index()
        {
            var model = new SupplierSearchModel();

            model.SetGridPageSize();

            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Index.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> List(SupplierSearchModel searchModel)
        {
            var model = await _supplierModelFactory.PrepareSupplierListModelAsync(searchModel);
            return Json(model);
        }







        public IActionResult Create()
        {
            var model = new SupplierModel(); // Initialize an empty SupplierModel
            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Create.cshtml", model);
        }


        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public async Task<IActionResult> Create(SupplierModel model, bool continueEditing)
        {
            try
            {
                if (ModelState.IsValid && !String.IsNullOrEmpty(model.Name))
                {
                    var supplierEntity = _supplierModelFactory.PrepareEntity(model);    

                    // Insert the new Supplier into the database
                    await _supplierService.InsertAsync(supplierEntity);

                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Supplier.Added"));

                    if (!continueEditing)
                        return RedirectToAction("Index");

                    return RedirectToAction("Edit", new { id = supplierEntity.Id });
                } 
                // If the model state is not valid, return the same view with the current model
                return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Create.cshtml", model);
            }catch(Exception e)
            {
                throw (e);
            }

           
        }


        // GET: Supplier/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            // Fetch the SupplierEntity by ID
            var supplierEntity = await _supplierService.GetByIdAsync(id);

            if (supplierEntity == null)
            {
                // Handle case where supplier is not found (e.g., 404)
                return NotFound();
            }

            // SupplierEntity to SupplierModel
            var supplierModel = _supplierModelFactory.PrepareModel(supplierEntity);

            // Return the Edit view with the SupplierModel
            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Edit.cshtml", supplierModel);
        }

        // POST: Supplier/Edit/{id}
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Edit(SupplierModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var supplierEntity = _supplierModelFactory.PrepareEntity(model);

                // Update the SupplierEntity in the database
                await _supplierService.UpdateAsync(supplierEntity);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.Updated"));


                if (!continueEditing)
                    return RedirectToAction("Index");

                return RedirectToAction("Edit", new { id = supplierEntity.Id });
            }

            // If the model state is invalid, return the Edit view with the model data
            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Edit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var supplierEntity = await _supplierService.GetByIdAsync(id);
            if (supplierEntity != null)
                await _supplierService.DeleteAsync(supplierEntity);

            return RedirectToAction("Index");
        }
    }
}
