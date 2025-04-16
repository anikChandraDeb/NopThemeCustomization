using System.Threading.Tasks;
using Nop.Plugin.Misc.Supplier.Model;

namespace Nop.Plugin.Misc.Supplier.Factories
{
    public interface ISupplierModelFactory
    {
        Task<SupplierListModel> PrepareSupplierListModelAsync(SupplierSearchModel searchModel);
    }
}
