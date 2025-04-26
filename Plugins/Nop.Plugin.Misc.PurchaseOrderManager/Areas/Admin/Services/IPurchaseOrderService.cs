using Nop.Core;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Domain;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Services;
public interface IPurchaseOrderService
{
    Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id);
    Task<IList<PurchaseOrder>> GetAllPurchaseOrdersAsync();
    Task<IPagedList<PurchaseOrder>> SearchPurchaseOrdersAsync(
        int supplierId = 0,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageIndex = 0,
        int pageSize = int.MaxValue);

}