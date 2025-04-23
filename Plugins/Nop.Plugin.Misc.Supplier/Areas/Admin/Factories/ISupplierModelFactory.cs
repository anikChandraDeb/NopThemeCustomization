using Nop.Plugin.Misc.Supplier.Areas.Admin.Domain;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Model;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Factories
{
    public interface ISupplierModelFactory
    {
        public SupplierEntity PrepareEntity(SupplierModel model);
        public SupplierModel PrepareModel(SupplierEntity entity);
        Task<SupplierListModel> PrepareSupplierListModelAsync(SupplierSearchModel searchModel);
        SupplierSearchModel PrepareSupplierSearchModel();
        Task<SupplierModel> PrepareCreateSupplierModelAsync();
        Task SaveLocalizedValuesAsync(SupplierEntity entity, IList<SupplierLocalizedModel> locales);
        Task<SupplierModel> PrepareEditModelAsync(SupplierEntity entity);
    }
}
