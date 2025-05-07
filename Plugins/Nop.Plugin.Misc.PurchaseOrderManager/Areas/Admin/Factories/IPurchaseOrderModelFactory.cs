using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models;
using Nop.Plugin.Misc.PurchaseOrderManager.Domain;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Factories;
public interface IPurchaseOrderModelFactory
{
    Task<PurchaseOrderSearchModel> PreparePurchaseOrderSearchModelAsync(PurchaseOrderSearchModel searchModel);
    Task<PurchaseOrderListModel> PreparePurchaseOrderListModelAsync(PurchaseOrderSearchModel searchModel);
    Task<PurchaseOrderModel> PreparePurchaseOrderModelAsync(PurchaseOrderModel model, PurchaseOrder purchaseOrder);
    Task<PurchaseOrderModel> PreparePurchaseOrderWithSuppliersModelAsync();
    Task<ProductListModel> PrepareSupplierProductListModelAsync(AddProductToPurchaseOrderSearchModel searchModel);
    Task<PurchaseOrder> PreparePurchaseOrderAsync(PurchaseOrderModel model);
    List<PurchaseOrderProduct> PrepareOrderItems(int purchaseOrderId, IList<PurchaseOrderItemModel> items);
    Task<AddProductToPurchaseOrderSearchModel> PrepareAddProductPopupModelAsync(int supplierId);
    Task<IList<PurchaseOrderItemModel>> PrepareTempOrderItemsAsync(AddProductsRequest request);
    Task<PurchaseOrderModel> PreparePurchaseOrderModelAsync(int id);
    Task<PurchaseOrderItemListModel> PreparePurchaseOrderItemListModelAsync(IList<PurchaseOrderItemModel> items, PurchaseOrderItemSearchModel searchModel);
}
