using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models
{
    public record PurchaseOrderModel : BaseNopEntityModel
    {
        public PurchaseOrderModel()
        {
            AvailableSuppliers = new List<SelectListItem>();
            Items = new List<PurchaseOrderItemModel>();
            AddProductSearchModel = new AddProductToPurchaseOrderSearchModel();
        }


        [NopResourceDisplayName("Admin.PurchaseOrders.Fields.OrderDate")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [NopResourceDisplayName("Admin.PurchaseOrders.Fields.Supplier")]
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public IList<SelectListItem> AvailableSuppliers { get; set; }

        [NopResourceDisplayName("Admin.PurchaseOrders.Fields.TotalAmount")]
        public decimal TotalAmount { get; set; }
        public string CreatedBy { get; set; }
        public int CreatedById { get; set; }
        public IList<PurchaseOrderItemModel> Items { get; set; }
        public AddProductToPurchaseOrderSearchModel? AddProductSearchModel { get; set; }
    }

    public record PurchaseOrderItemModel : BaseNopEntityModel
    {
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitCost { get; set; }
        public decimal LineTotal { get; set; }
    }

    public record PurchaseOrderItemListModel: BasePagedListModel<PurchaseOrderItemModel>
    {
    }

    public record AddProductToPurchaseOrderSearchModel : BaseSearchModel
    {
        public int SupplierId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
        public string SearchProductName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchSku")]
        public string SearchSku { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
        public int SearchCategoryId { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();

        public IList<int> SelectedProductIds { get; set; } = new List<int>();
    }

    

}