using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Domain;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Services;
using Nop.Plugin.Misc.PurchaseOrderManager.Models;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Factories
{
    public class PurchaseOrderModelFactory : IPurchaseOrderModelFactory
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly ISupplierService _supplierService;
        private readonly ICustomerService _customerService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IProductService _productService;

        public PurchaseOrderModelFactory(
            IPurchaseOrderService purchaseOrderService,
            ISupplierService supplierService,
            ICustomerService customerService,
            IStaticCacheManager staticCacheManager,
            IProductService productService)
        {
            _purchaseOrderService = purchaseOrderService;
            _supplierService = supplierService;
            _customerService = customerService;
            _staticCacheManager = staticCacheManager;
            _productService = productService;
        }

        public async Task<PurchaseOrderSearchModel> PreparePurchaseOrderSearchModelAsync(PurchaseOrderSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new PurchaseOrderSearchModel();

            // Populate AvailableSuppliers
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            foreach (var supplier in suppliers)
            {
                searchModel.AvailableSuppliers.Add(new SelectListItem
                {
                    Text = supplier.Name,
                    Value = supplier.Id.ToString()
                });
            }

            searchModel.SetGridPageSize();
            return searchModel;
        }

        public async Task<PurchaseOrderListModel> PreparePurchaseOrderListModelAsync(PurchaseOrderSearchModel searchModel)
        {
            var purchaseOrders = await _purchaseOrderService.SearchPurchaseOrdersAsync(
                supplierId: searchModel.SupplierId,
                startDate: searchModel.StartDate,
                endDate: searchModel.EndDate,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = await new PurchaseOrderListModel().PrepareToGridAsync(searchModel, purchaseOrders, () =>
            {
                return purchaseOrders.SelectAwait(async po =>
                {
                    var supplier = await _supplierService.GetByIdAsync(po.SupplierId);
                    var createdBy = await _customerService.GetCustomerByIdAsync(po.CreatedById);

                    return new PurchaseOrderModel
                    {
                        Id = po.Id,
                        SupplierId = po.SupplierId,
                        SupplierName = supplier?.Name ?? "N/A",
                        OrderDate = po.CreatedOnUtc,
                        CreatedBy = createdBy?.Email ?? "System",
                        TotalAmount = po.TotalAmount
                    };
                });
            });

            return model;
        }

        public async Task<PurchaseOrderModel> PreparePurchaseOrderModelAsync(PurchaseOrderModel model, PurchaseOrder purchaseOrder)
        {
            if (purchaseOrder != null)
            {
                model ??= new PurchaseOrderModel();
                model.Id = purchaseOrder.Id;
                model.SupplierId = purchaseOrder.SupplierId;
                model.OrderDate = purchaseOrder.CreatedOnUtc;
                model.TotalAmount = purchaseOrder.TotalAmount;
                model.CreatedById = purchaseOrder.CreatedById;

                var supplier = await _supplierService.GetByIdAsync(purchaseOrder.SupplierId);
                if (supplier != null)
                {
                    model.SupplierName = supplier.Name;
                }

                var createdBy = await _customerService.GetCustomerByIdAsync(purchaseOrder.CreatedById);
                if (createdBy != null)
                {
                    model.CreatedBy = createdBy.Email;
                }
            }

            return model;
        }

        public async Task<PurchaseOrderModel> PreparePurchaseOrderWithSuppliersModelAsync()
        {
            // Create the PurchaseOrderModel instance
            var model = new PurchaseOrderModel
            {
                OrderDate = DateTime.UtcNow
            };

            // Fetch the list of suppliers
            var suppliers = await _supplierService.GetAllSuppliersAsync();

            // Populate AvailableSuppliers
            model.AvailableSuppliers = suppliers.Select(supplier => new SelectListItem
            {
                Value = supplier.Id.ToString(),
                Text = supplier.Name
            }).ToList();

            // Retrieve purchase order items from session
            var items = _purchaseOrderService.GetSessionItems(); // Assuming it returns List<PurchaseOrderItemModel>
            model.TotalAmount = items.Sum(item => item.LineTotal);

            // Populate Items property
            //model.Items = items;

            // Clear session items after assigning to model
            //_purchaseOrderService.ClearSessionItems();
            return model;
        }



        public async Task<ProductListModel> PrepareSupplierProductListModelAsync(AddProductToPurchaseOrderSearchModel searchModel)
        {
            // Get product IDs for the selected supplier
            var supplierProductIds = await _purchaseOrderService.GetProductIdBySupplierIdAsync(searchModel.SupplierId);

            // Search all products matching filter criteria
            var allProducts = await _productService.SearchProductsAsync(
                categoryIds: searchModel.SearchCategoryId > 0 ? new List<int> { searchModel.SearchCategoryId } : null,
                keywords: !string.IsNullOrEmpty(searchModel.SearchSku) ? searchModel.SearchSku : searchModel.SearchProductName,
                searchSku: !string.IsNullOrEmpty(searchModel.SearchSku),
                pageIndex: 0, // fetch all for manual filtering
                pageSize: int.MaxValue
            );

            // Filter only supplier's products
            var filteredProducts = allProducts.Where(p => supplierProductIds.Contains(p.Id)).ToList();

            // Paginate manually
            var pagedProducts = new PagedList<Product>(
                filteredProducts.Skip((searchModel.Page - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList(),
                searchModel.Page - 1,
                searchModel.PageSize,
                filteredProducts.Count
            );

            // Convert to ProductModel
            var productModels = await pagedProducts.SelectAwait(async product =>
            {
                var productModel = new ProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Sku = product.Sku,
                    StockQuantity = product.StockQuantity,
                    Price = product.Price
                };
                return productModel;
            }).ToListAsync();

            var pagedProductModels = new PagedList<ProductModel>(
                productModels,
                pagedProducts.PageIndex,
                pagedProducts.PageSize,
                pagedProducts.TotalCount);

            // Prepare grid
            var model = await new ProductListModel().PrepareToGridAsync(
                searchModel,
                pagedProductModels,
                () => pagedProductModels.ToAsyncEnumerable());

            return model;
        }



        //public async Task<PurchaseOrderItemsListModel> PreparePurchaseOrderItemsListModelAsync(int purchaseOrderId)
        //{
        //    var items = await _purchaseOrderService.GetItemsByPurchaseOrderIdAsync(purchaseOrderId);

        //    var model = await ModelExtensions.PrepareToGridAsync<PurchaseOrderItemsListModel, PurchaseOrderItemModel, PurchaseOrderItemModel>(
        //        new PurchaseOrderItemsListModel(),
        //        new AddProductToPurchaseOrderSearchModel(), // Adjust this model as necessary
        //        items,
        //        () =>
        //        {
        //            return items.Select(item => new PurchaseOrderItemModel
        //            {
        //                Id = item.Id,
        //                ProductName = item.ProductName,
        //                Sku = item.Sku,
        //                Quantity = item.Quantity,
        //                UnitCost = item.UnitCost,
        //                LineTotal = item.LineTotal
        //            }).ToAsyncEnumerable();
        //        });

        //    return model;
        //}


    }
}