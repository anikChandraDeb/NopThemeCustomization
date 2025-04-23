using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Domain;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IRepository<SupplierEntity> _repository;
        private readonly IRepository<ProductSupplierMapping> _productSupplierMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        public SupplierService(IRepository<SupplierEntity> repository, 
            IRepository<ProductSupplierMapping> productSupplierMappingRepository,
            IStaticCacheManager staticCacheManager)
        {
            _repository = repository;
            _productSupplierMappingRepository = productSupplierMappingRepository;
            _staticCacheManager = staticCacheManager;
        }

        public async Task InsertAsync(SupplierEntity supplier)
        {
            await _repository.InsertAsync(supplier);
            await _staticCacheManager.RemoveByPrefixAsync(SupplierDefaults.AdminSupplierAllPrefixCacheKey);
        }

        public async Task UpdateAsync(SupplierEntity supplier)
        {
            await _repository.UpdateAsync(supplier);
            await _staticCacheManager.RemoveByPrefixAsync(SupplierDefaults.AdminSupplierAllPrefixCacheKey);
        }

        public async Task DeleteAsync(SupplierEntity supplier)
        {
            await _repository.DeleteAsync(supplier);
            await _staticCacheManager.RemoveByPrefixAsync(SupplierDefaults.AdminSupplierAllPrefixCacheKey);

        }

        public async Task<SupplierEntity> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<IList<SupplierEntity>> GetAllSuppliersAsync() =>  await _repository.Table.ToListAsync();
        public async Task<IPagedList<SupplierEntity>> GetAllAsync(string name, string email, int pageIndex, int pageSize)
        {
            name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
            email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(SupplierDefaults.AdminSupplierAllModelKey,
                            name, email, pageIndex, pageSize);


            var allSuppliers = await _staticCacheManager.GetAsync<IList<SupplierEntity>>(cacheKey, async () =>
            {
                var query = _repository.Table;

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(s => s.Name.Contains(name));

                if (!string.IsNullOrEmpty(email))
                    query = query.Where(s => s.Email.Contains(email));

                return await query.OrderBy(s => s.Name).ToListAsync();
            });

            return new PagedList<SupplierEntity>(allSuppliers, pageIndex, pageSize);
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
