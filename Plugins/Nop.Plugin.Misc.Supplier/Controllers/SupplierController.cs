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
using Nop.Web.Framework.Factories;
using System.Text.RegularExpressions;

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
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ILocalizedEntityService _localizedEntityService;
        public SupplierController(
            ISupplierService supplierService,
            IPermissionService permissionService,
            ISupplierModelFactory supplierModelFactory,
            INotificationService notificationService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            ILocalizedEntityService localizedEntityService
            )
        {
            _supplierService = supplierService;
            _permissionService = permissionService;
            _supplierModelFactory = supplierModelFactory;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _localizedEntityService = localizedEntityService;
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


        public async Task<IActionResult> Create()
        {
            var model = new SupplierModel();

            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync<SupplierLocalizedModel>(
                async (locale, languageId) =>
                {
                    locale.LanguageId = languageId;
                });

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
                    model.Description = StripPTags(model.Description);

                    var supplierEntity = _supplierModelFactory.PrepareEntity(model);    

                    await _supplierService.InsertAsync(supplierEntity);

                    foreach (var localized in model.Locales)
                    {
                        await _localizedEntityService.SaveLocalizedValueAsync(supplierEntity, x => x.Name, localized.Name, localized.LanguageId);
                        await _localizedEntityService.SaveLocalizedValueAsync(supplierEntity, x => x.Address, localized.Address, localized.LanguageId);
                        await _localizedEntityService.SaveLocalizedValueAsync(supplierEntity, x => x.Description, localized.Description, localized.LanguageId);
                    }

                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Supplier.Added"));

                    if (!continueEditing)
                        return RedirectToAction("Index");

                    return RedirectToAction("Edit", new { id = supplierEntity.Id });
                } 
                
                return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Create.cshtml", model);
            }catch(Exception e)
            {
                throw (e);
            }

           
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplierEntity = await _supplierService.GetByIdAsync(id);

            if (supplierEntity == null)
            {
                return NotFound();
            }

            var supplierModel = _supplierModelFactory.PrepareModel(supplierEntity);

            supplierModel.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync<SupplierLocalizedModel>(
            async (locale, languageId) =>
            {
                locale.LanguageId = languageId;
                locale.Name = await _localizationService.GetLocalizedAsync(supplierEntity, x => x.Name, languageId, false, false);
                locale.Description = await _localizationService.GetLocalizedAsync(supplierEntity, x => x.Description, languageId, false, false);

            });

            return View("~/Plugins/Nop.Plugin.Misc.Supplier/Views/Supplier/Edit.cshtml", supplierModel);
        }


        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Edit(SupplierModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                model.Description = StripPTags(model.Description);

                var supplierEntity = _supplierModelFactory.PrepareEntity(model);

                
                await _supplierService.UpdateAsync(supplierEntity);

                
                foreach (var localized in model.Locales)
                {
                    await _localizedEntityService.SaveLocalizedValueAsync(supplierEntity, x => x.Name, localized.Name, localized.LanguageId);
                    await _localizedEntityService.SaveLocalizedValueAsync(supplierEntity, x => x.Address, localized.Address, localized.LanguageId);
                    await _localizedEntityService.SaveLocalizedValueAsync(supplierEntity, x => x.Description, localized.Description, localized.LanguageId);
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.Updated"));


                if (!continueEditing)
                    return RedirectToAction("Index");

                return RedirectToAction("Edit", new { id = supplierEntity.Id });
            }

            
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

        public static string StripPTags(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remove wrapping <p> and </p> tags only
            return Regex.Replace(input, @"^<p>(.*?)</p>$", "$1", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
    }
}
