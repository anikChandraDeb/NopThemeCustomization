using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Model;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IRepository<SupplierEntity> _repository;
        private readonly IRepository<ProductSupplierMapping> _productSupplierMappingRepository;

        public SupplierService(IRepository<SupplierEntity> repository, IRepository<ProductSupplierMapping> productSupplierMappingRepository)
        {
            _repository = repository;
            _productSupplierMappingRepository = productSupplierMappingRepository;
        }

        public async Task InsertAsync(SupplierEntity supplier) => await _repository.InsertAsync(supplier);
        public async Task UpdateAsync(SupplierEntity supplier) => await _repository.UpdateAsync(supplier);
        public async Task DeleteAsync(SupplierEntity supplier) => await _repository.DeleteAsync(supplier);
        public async Task<SupplierEntity> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<IList<SupplierEntity>> GetAllSuppliersAsync()
        {
            return await _repository.Table.ToListAsync();
        }
        public async Task<IPagedList<SupplierEntity>> GetAllAsync(string name, string email, int pageIndex, int pageSize)
        {
            var query = _repository.Table;

            if (!string.IsNullOrEmpty(name))
                query = query.Where(s => s.Name.Contains(name));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(s => s.Email.Contains(email));

            query = query.OrderBy(s => s.Name);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        public async Task InsertOrUpdateProductSupplierMappingAsync(int productId, int supplierId)
        {
            var existing = await _productSupplierMappingRepository.Table
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (existing != null)
            {
                existing.SupplierId = supplierId;
                await _productSupplierMappingRepository.UpdateAsync(existing);
            }
            else
            {
                var newMapping = new ProductSupplierMapping
                {
                    ProductId = productId,
                    SupplierId = supplierId
                };
                await _productSupplierMappingRepository.InsertAsync(newMapping);
            }
        }

        public async Task<int> GetProductSupplierIdAsync(int productId)
        {
            var existing = await _productSupplierMappingRepository.Table
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (existing != null)
            {
                return existing.SupplierId;
            }
            else return 0;
        }


    }
}
