using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Domain;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models;
using Nop.Plugin.Misc.PurchaseOrderManager.Models;
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
