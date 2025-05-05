using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Utility;
public static class PurchaseOrderLocaleResources
{
    public static Dictionary<string, string> GetAll() => new Dictionary<string, string>
    {
        // Page Titles
        ["Admin.PurchaseOrders"] = "Purchase Orders",
        ["Admin.PurchaseOrders.AddNew"] = "Create Purchase Order",
        ["Admin.PurchaseOrders.Info"] = "Purchase Order Information",
        ["Admin.PurchaseOrders.BackToList"] = "Back to purchase orders list",
        ["Admin.PurchaseOrder.Added"] = "Purchase order added successfully",
        ["Admin.PurchaseOrder.Updated"] = "Purchase order updated successfully",
        ["Admin.PurchaseOrder.Deleted"] = "Purchase order deleted successfully",
        ["Admin.PurchaseOrders.EditDetails"] = "Edit Purchase Order Details",

        // Validation Messages
        ["Admin.PurchaseOrders.Fields.Supplier.Required"] = "Supplier is required",
        ["Admin.PurchaseOrders.Fields.Product.Required"] = "At least one product is required",
        ["Admin.PurchaseOrders.Fields.Quantity.Required"] = "Quantity is required",
        ["Admin.PurchaseOrders.Fields.Quantity.Min"] = "Quantity must be at least 1",
        ["Admin.PurchaseOrders.Fields.UnitCost.Required"] = "Unit cost is required",
        ["Admin.PurchaseOrders.Fields.UnitCost.Min"] = "Unit cost must be positive",

        // Search Fields
        ["Admin.PurchaseOrders.List.SearchSupplier"] = "Supplier",
        ["Admin.PurchaseOrders.List.SearchSupplier.Hint"] = "Search by supplier name",
        ["Admin.PurchaseOrders.List.SearchStartDate"] = "Date from",
        ["Admin.PurchaseOrders.List.SearchStartDate.Hint"] = "Search orders created after this date",
        ["Admin.PurchaseOrders.List.SearchEndDate"] = "Date to",
        ["Admin.PurchaseOrders.List.SearchEndDate.Hint"] = "Search orders created before this date",

        // Form Fields
        ["Admin.PurchaseOrders.Fields.Id"] = "Order#",
        ["Admin.PurchaseOrders.Fields.CreatedOnUtc"] = "Created on",
        ["Admin.PurchaseOrders.Fields.Supplier"] = "Supplier",
        ["Admin.PurchaseOrders.Fields.Supplier.Hint"] = "Select the supplier for this order",
        ["Admin.PurchaseOrders.Fields.OrderDate"] = "Order Date",
        ["Admin.PurchaseOrders.Fields.OrderDate.Hint"] = "Date when the order was created",
        ["Admin.PurchaseOrders.Fields.TotalAmount"] = "Total Amount",
        ["Admin.PurchaseOrders.Fields.TotalAmount.Hint"] = "Calculated total amount for the order",
        ["Admin.PurchaseOrders.Fields.CreatedBy"] = "Created By",
        ["Admin.PurchaseOrders.Fields.CreatedBy.Hint"] = "User who created this order",
        ["Admin.PurchaseOrders.Fields.Status"] = "Status",
        ["Admin.PurchaseOrders.Fields.Status.Hint"] = "Current status of the order",

        // Product Fields
        ["Admin.PurchaseOrders.Fields.Product"] = "Product",
        ["Admin.PurchaseOrders.Fields.Product.Name"] = "Product",
        ["Admin.PurchaseOrders.Fields.Product.Name.Hint"] = "Select product to order",
        ["Admin.PurchaseOrders.Fields.SKU"] = "SKU",
        ["Admin.PurchaseOrders.Fields.Quantity"] = "Quantity",
        ["Admin.PurchaseOrders.Fields.Quantity.Hint"] = "Enter quantity to order",
        ["Admin.PurchaseOrders.Fields.UnitCost"] = "Unit Cost",
        ["Admin.PurchaseOrders.Fields.UnitCost.Hint"] = "Cost per unit from supplier",
        ["Admin.PurchaseOrders.Fields.LineTotal"] = "Line Total",
        ["Admin.PurchaseOrders.Fields.LineTotal.Hint"] = "Quantity × Unit Cost",

        // Actions
        ["Admin.PurchaseOrders.AddProduct"] = "Add Product",
        ["Admin.PurchaseOrders.RemoveProduct"] = "Remove",
        ["Admin.PurchaseOrders.Save"] = "Save Order",
        ["Admin.PurchaseOrders.Cancel"] = "Cancel",
        ["Admin.PurchaseOrders.Edit"] = "Edit",

        // Widget/Help Text
        ["Admin.PurchaseOrder.Widget"] = "Purchase Orders",
        ["Admin.PurchaseOrder.Widget.Description"] = "Purchase orders allow you to track inventory purchases from suppliers. Create purchase orders to manage your inventory replenishment and maintain records of supplier transactions.",
        ["Admin.PurchaseOrder.Widget.Message"] = "You need to select a supplier before adding products to the order",
        ["Admin.Purchaseorders.Items"]="Purchase Order Products",
        ["Admin.Catalog.Products.Fields.Sku"] ="SKU"
    };
}
