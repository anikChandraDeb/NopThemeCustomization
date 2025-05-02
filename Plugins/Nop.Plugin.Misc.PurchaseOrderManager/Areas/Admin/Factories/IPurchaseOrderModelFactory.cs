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
}
