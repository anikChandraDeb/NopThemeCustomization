using Nop.Data;
using Nop.Plugin.Misc.Supplier.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Supplier.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IRepository<SupplierEntity> _repository;

        public SupplierService(IRepository<SupplierEntity> repository)
        {
            _repository = repository;
        }

        public async Task InsertAsync(SupplierEntity supplier) => await _repository.InsertAsync(supplier);
        public async Task UpdateAsync(SupplierEntity supplier) => await _repository.UpdateAsync(supplier);
        public async Task DeleteAsync(SupplierEntity supplier) => await _repository.DeleteAsync(supplier);
        public async Task<SupplierEntity> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task<IList<SupplierEntity>> GetAllAsync() => await _repository.GetAllAsync(
        (Func<IQueryable<SupplierEntity>, IQueryable<SupplierEntity>>)(query => query));
    }
}
