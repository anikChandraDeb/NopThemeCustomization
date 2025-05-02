using Nop.Core;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models;
using Nop.Plugin.Misc.PurchaseOrderManager.Domain;
using Nop.Web.Areas.Admin.Models.Catalog;
namespace Nop.Plugin.Misc.PurchaseOrderManager.Services;
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
    Task<List<int>> GetProductIdBySupplierIdAsync(int supplierId);
    Task<IList<PurchaseOrderItemModel>> GetItemsByOrderIdAsync(int orderId);
    Task UpdateProductStockQuantity(ProductModel product);
    Task AssignProductStockQuantity(ProductModel product);
    Task AddTempPurchaseOrderItemAsync(PurchaseOrderItemModel item);
    Task<List<PurchaseOrderItemModel>> GetTempPurchaseOrderItemsAsync();
    List<PurchaseOrderItemModel> GetSessionItems();
    void SaveSessionItems(List<PurchaseOrderItemModel> items);
    Task ClearSessionItems();
    Task<ProductModel> GetProductByIdAsync(int id);
}