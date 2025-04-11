using Nop.Plugin.Misc.Supplier.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Supplier.Services
{
    public interface ISupplierService
    {
        Task InsertAsync(SupplierEntity supplier);
        Task UpdateAsync(SupplierEntity supplier);
        Task DeleteAsync(SupplierEntity supplier);
        Task<SupplierEntity> GetByIdAsync(int id);
        Task<IList<SupplierEntity>> GetAllAsync();
    }
}
