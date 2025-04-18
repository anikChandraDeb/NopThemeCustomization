using System.Threading.Tasks;
using Nop.Plugin.Misc.Supplier.Domain;
using Nop.Plugin.Misc.Supplier.Model;

namespace Nop.Plugin.Misc.Supplier.Factories
{
    public interface ISupplierModelFactory
    {
        public SupplierEntity PrepareEntity(SupplierModel model);
        public SupplierModel PrepareModel(SupplierEntity entity);
        Task<SupplierListModel> PrepareSupplierListModelAsync(SupplierSearchModel searchModel);
    }
}
