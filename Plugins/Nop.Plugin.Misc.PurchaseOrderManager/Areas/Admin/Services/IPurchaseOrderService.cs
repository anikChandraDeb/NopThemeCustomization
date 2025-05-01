using Nop.Core;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Domain;
using Nop.Plugin.Misc.PurchaseOrderManager.Models;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Services;
public interface IPurchaseOrderService
{
    Task InsertPurchaseOrderAsync(PurchaseOrder purchaseOrder);
    Task UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
    Task DeletePurchaseOrderAsync(PurchaseOrder purchaseOrder);
    Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id);
    Task<IList<PurchaseOrder>> GetAllPurchaseOrdersAsync();
    Task<IPagedList<PurchaseOrder>> SearchPurchaseOrdersAsync(
        int supplierId = 0,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageIndex = 0,
        int pageSize = int.MaxValue);

    Task InsertPurchaseOrderProductAsync(PurchaseOrderProduct product);
    Task UpdatePurchaseOrderProductAsync(PurchaseOrderProduct product);
    Task DeletePurchaseOrderProductAsync(PurchaseOrderProduct product);
    Task<PurchaseOrderProduct> GetPurchaseOrderProductByIdAsync(int id);
    Task<IList<PurchaseOrderProduct>> GetProductsByPurchaseOrderIdAsync(int purchaseOrderId);
    Task<IPagedList<ProductModel>> GetProductsBySupplierIdAsync(int supplierId, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<IList<PurchaseOrderItemModel>> GetItemsByOrderIdAsync(int orderId);
    Task UpdateProductStockQuantity(ProductModel product);
    Task AssignProductStockQuantity(ProductModel product);
    Task AddTempPurchaseOrderItemAsync(PurchaseOrderItemModel item);
    Task<List<PurchaseOrderItemModel>> GetTempPurchaseOrderItemsAsync();
    List<PurchaseOrderItemModel> GetSessionItems();
    void SaveSessionItems(List<PurchaseOrderItemModel> items);
    void ClearSessionItems();
    Task<ProductModel> GetProductByIdAsync(int id);
}