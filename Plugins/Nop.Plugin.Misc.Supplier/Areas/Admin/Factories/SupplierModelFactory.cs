using Nop.Plugin.Misc.Supplier.Areas.Admin.Domain;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Model;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Services;
using Nop.Services.Localization;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Factories
{
    public class SupplierModelFactory : ISupplierModelFactory
    {
        private readonly ISupplierService _supplierService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;

        public SupplierModelFactory(ISupplierService supplierService,
            ILocalizedModelFactory localizedModelFactory,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService)
        {
            _supplierService = supplierService;
            _localizedModelFactory = localizedModelFactory;
            _localizedEntityService = localizedEntityService;
            _localizationService = localizationService;
        }

        public SupplierSearchModel PrepareSupplierSearchModel()
        {
            var model = new SupplierSearchModel();
            model.SetGridPageSize();
            return model;
        }

        public async Task<SupplierModel> PrepareCreateSupplierModelAsync()
        {
            var model = new SupplierModel();

            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync<SupplierLocalizedModel>(
                async (locale, languageId) =>
                {
                    locale.LanguageId = languageId;
                });

            return model;
        }

        public async Task SaveLocalizedValuesAsync(SupplierEntity entity, IList<SupplierLocalizedModel> locales)
        {
            foreach (var localized in locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Name, localized.Name, localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Address, localized.Address, localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(entity, x => x.Description, localized.Description, localized.LanguageId);
            }
        }


        public async Task<SupplierModel> PrepareEditModelAsync(SupplierEntity entity)
        {
            var model = PrepareModel(entity); // Already maps base fields

            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync<SupplierLocalizedModel>(
                async (locale, languageId) =>
                {
                    locale.LanguageId = languageId;
                    locale.Name = await _localizationService.GetLocalizedAsync(entity, x => x.Name, languageId, false, false);
                    locale.Description = await _localizationService.GetLocalizedAsync(entity, x => x.Description, languageId, false, false);
                    locale.Address = await _localizationService.GetLocalizedAsync(entity, x => x.Address, languageId, false, false);
                });

            return model;
        }


        public SupplierEntity PrepareEntity(SupplierModel model)
        {
            return new SupplierEntity
            {
                Id = model.Id,
                Name = model.Name,
                ContactPerson = model.ContactPerson,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                Description = model.Description,  
                IsActive = model.IsActive         
            };
        }

        public SupplierModel PrepareModel(SupplierEntity entity)
        {
            return new SupplierModel
            {
                Id = entity.Id,
                Name = entity.Name,
                ContactPerson = entity.ContactPerson,
                Phone = entity.Phone,
                Email = entity.Email,
                Address = entity.Address,
                Description = entity.Description,
                IsActive = entity.IsActive        
            };
        }

        public async Task<SupplierListModel> PrepareSupplierListModelAsync(SupplierSearchModel searchModel)
        {
            var suppliers = await _supplierService.GetAllAsync(
                searchModel.SearchName,
                searchModel.SearchEmail,
                searchModel.Page - 1,
                searchModel.PageSize
            );

            var model = await new SupplierListModel().PrepareToGridAsync(searchModel, suppliers, () =>
            {
                return suppliers.Select(supplier => new SupplierModel
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    ContactPerson = supplier.ContactPerson,
                    Phone = supplier.Phone,
                    Email = supplier.Email,
                    Address = supplier.Address,
                    Description = supplier.Description,
                    IsActive = supplier.IsActive       
                }).ToAsyncEnumerable();
            });

            return model;
        }
    }

}
