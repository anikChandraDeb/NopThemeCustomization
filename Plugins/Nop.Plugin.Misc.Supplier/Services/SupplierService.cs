using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.Supplier.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services;

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
        public async Task<IPagedList<SupplierEntity>> GetAllAsync(string name, string email, int pageIndex, int pageSize)
        {
            var query = _repository.Table;

            // Apply filters if the name or email is provided
            if (!string.IsNullOrEmpty(name))
                query = query.Where(s => s.Name.Contains(name));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(s => s.Email.Contains(email));

            // Apply pagination and ordering
            query = query.OrderBy(s => s.Name); // Change this to another field if necessary

            // Return the paginated list using the ToPagedList method
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }



    }
}
