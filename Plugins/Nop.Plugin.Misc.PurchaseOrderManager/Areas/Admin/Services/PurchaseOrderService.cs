using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Domain;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Services;
public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;
    private readonly IRepository<PurchaseOrderProduct> _purchaseOrderProductRepository;
    private readonly IStaticCacheManager _staticCacheManager;

    public PurchaseOrderService(
        IRepository<PurchaseOrder> purchaseOrderRepository,
        IRepository<PurchaseOrderProduct> purchaseOrderProductRepository,
        IStaticCacheManager staticCacheManager)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _purchaseOrderProductRepository = purchaseOrderProductRepository;
        _staticCacheManager = staticCacheManager;
    }
    public async Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id)
    {
        if (id == 0)
            return null;

        return await _purchaseOrderRepository.GetByIdAsync(id);
    }

    public async Task<IList<PurchaseOrder>> GetAllPurchaseOrdersAsync()
    {
        return await _purchaseOrderRepository.Table.ToListAsync();
    }

    public async Task<IPagedList<PurchaseOrder>> SearchPurchaseOrdersAsync(
        int supplierId = 0,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageIndex = 0,
        int pageSize = int.MaxValue)
    {
        var query = _purchaseOrderRepository.Table;

        if (supplierId > 0)
            query = query.Where(po => po.SupplierId == supplierId);

        if (startDate.HasValue)
            query = query.Where(po => po.CreatedOnUtc >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(po => po.CreatedOnUtc <= endDate.Value.Date.AddDays(1));

        query = query.OrderByDescending(po => po.CreatedOnUtc);

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }
}
