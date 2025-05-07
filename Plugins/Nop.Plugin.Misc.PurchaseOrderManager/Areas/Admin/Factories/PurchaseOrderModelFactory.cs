using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models;
using Nop.Plugin.Misc.PurchaseOrderManager.Domain;
using Nop.Plugin.Misc.PurchaseOrderManager.Services;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
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
        private readonly IWorkContext _workContext;
        private readonly ICategoryService _categoryService;
        private readonly ILocalizationService _localizationService;

        public PurchaseOrderModelFactory(
            IPurchaseOrderService purchaseOrderService,
            ISupplierService supplierService,
            ICustomerService customerService,
            IStaticCacheManager staticCacheManager,
            IProductService productService,
            IWorkContext workContext,
            ICategoryService categoryService,
            ILocalizationService localizationService)
        {
            _purchaseOrderService = purchaseOrderService;
            _supplierService = supplierService;
            _customerService = customerService;
            _staticCacheManager = staticCacheManager;
            _productService = productService;
            _workContext = workContext;
            _categoryService = categoryService;
            _localizationService = localizationService;
        }

        public async Task<PurchaseOrderSearchModel> PreparePurchaseOrderSearchModelAsync(PurchaseOrderSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new PurchaseOrderSearchModel();

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
            var model = new PurchaseOrderModel
            {
                OrderDate = DateTime.UtcNow
            };

            var suppliers = await _supplierService.GetAllSuppliersAsync();

            model.AvailableSuppliers = suppliers.Select(supplier => new SelectListItem
            {
                Value = supplier.Id.ToString(),
                Text = supplier.Name
            }).ToList();

            var items = _purchaseOrderService.GetSessionItems(); // Assuming it returns List<PurchaseOrderItemModel>
            model.TotalAmount = items.Sum(item => item.LineTotal);

            return model;
        }



        public async Task<ProductListModel> PrepareSupplierProductListModelAsync(AddProductToPurchaseOrderSearchModel searchModel)
        {
            var supplierProductIds = await _purchaseOrderService.GetProductIdBySupplierIdAsync(searchModel.SupplierId);

            var allProducts = await _productService.SearchProductsAsync(
                categoryIds: searchModel.SearchCategoryId > 0 ? new List<int> { searchModel.SearchCategoryId } : null,
                keywords: !string.IsNullOrEmpty(searchModel.SearchSku) ? searchModel.SearchSku : searchModel.SearchProductName,
                searchSku: !string.IsNullOrEmpty(searchModel.SearchSku),
                pageIndex: 0, 
                pageSize: int.MaxValue
            );

            var filteredProducts = allProducts.Where(p => supplierProductIds.Contains(p.Id)).ToList();

            var pagedProducts = new PagedList<Product>(
                filteredProducts.Skip((searchModel.Page - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList(),
                searchModel.Page - 1,
                searchModel.PageSize,
                filteredProducts.Count
            );

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

            var model = await new ProductListModel().PrepareToGridAsync(
                searchModel,
                pagedProductModels,
                () => pagedProductModels.ToAsyncEnumerable());

            return model;
        }

        public async Task<PurchaseOrder> PreparePurchaseOrderAsync(PurchaseOrderModel model)
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();

            return new PurchaseOrder
            {
                SupplierId = model.SupplierId,
                CreatedOnUtc = model.OrderDate,
                TotalAmount = model.TotalAmount,
                CreatedById = currentCustomer.Id
            };
        }

        public List<PurchaseOrderProduct> PrepareOrderItems(int purchaseOrderId, IList<PurchaseOrderItemModel> items)
        {
            return items.Select(item => new PurchaseOrderProduct
            {
                PurchaseOrderId = purchaseOrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost,
                LineTotal = item.LineTotal
            }).ToList();
        }
        public async Task<AddProductToPurchaseOrderSearchModel> PrepareAddProductPopupModelAsync(int supplierId)
        {
            var model = new AddProductToPurchaseOrderSearchModel
            {
                SupplierId = supplierId,
                SelectedProductIds = new List<int>(),
                AvailablePageSizes = "10, 15, 20, 50, 100"
            };

            var categories = await _categoryService.GetAllCategoriesAsync(showHidden: true);
            foreach (var category in categories)
            {
                model.AvailableCategories.Add(new SelectListItem
                {
                    Text = category.Name,
                    Value = category.Id.ToString()
                });
            }

            model.AvailableCategories.Insert(0, new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Common.All"),
                Value = "0"
            });

            return model;
        }
        public async Task<IList<PurchaseOrderItemModel>> PrepareTempOrderItemsAsync(AddProductsRequest request)
        {
            var result = new List<PurchaseOrderItemModel>();

            if (request?.SelectedIds == null)
                return result;

            foreach (var productId in request.SelectedIds)
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                    continue;

                var quantity = request.Quantities?.TryGetValue(productId, out var qty) == true ? qty : 1;
                var unitCost = request.Prices?.TryGetValue(productId, out var price) == true ? price : 0;

                result.Add(new PurchaseOrderItemModel
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Sku = product.Sku,
                    Quantity = quantity,
                    UnitCost = unitCost,
                    LineTotal = quantity * unitCost
                });
            }

            return result;
        }
        public async Task<PurchaseOrderModel> PreparePurchaseOrderModelAsync(int id)
        {
            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);
            if (purchaseOrder == null)
                return null;

            var suppliers = await _supplierService.GetAllSuppliersAsync();
            var supplier = suppliers.FirstOrDefault(s => s.Id == purchaseOrder.SupplierId);
            var createdBy = await _customerService.GetCustomerByIdAsync(purchaseOrder.CreatedById);
            var items = await _purchaseOrderService.GetItemsByOrderIdAsync(purchaseOrder.Id);

            var model = new PurchaseOrderModel
            {
                Id = purchaseOrder.Id,
                SupplierId = purchaseOrder.SupplierId,
                SupplierName = supplier?.Name ?? "N/A",
                OrderDate = purchaseOrder.CreatedOnUtc,
                CreatedById = purchaseOrder.CreatedById,
                CreatedBy = createdBy?.Email ?? "System",
                AvailableSuppliers = suppliers.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList(),
                Items = items
            };

            _purchaseOrderService.SaveSessionItems((List<PurchaseOrderItemModel>)items);

            return model;
        }
        public async Task<PurchaseOrderItemListModel> PreparePurchaseOrderItemListModelAsync(IList<PurchaseOrderItemModel> items,PurchaseOrderItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            // Paging
            var pagedItems = new PagedList<PurchaseOrderItemModel>(
                items.Skip((searchModel.Page - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList(),
                searchModel.Page - 1,
                searchModel.PageSize,
                items.Count
            );

            // Mapping to model
            var itemModels = await pagedItems.SelectAwait(async item =>
            {
                var model = new PurchaseOrderItemModel
                {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    Sku = item.Sku,
                    ProductId=item.ProductId,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    LineTotal = item.Quantity * item.UnitCost
                };

                return model;
            }).ToListAsync();

            var pagedItemModels = new PagedList<PurchaseOrderItemModel>(
                itemModels,
                pagedItems.PageIndex,
                pagedItems.PageSize,
                pagedItems.TotalCount
            );

            // Prepare the final grid model
            var model = await new PurchaseOrderItemListModel().PrepareToGridAsync(
                searchModel,
                pagedItemModels,
                () => pagedItemModels.ToAsyncEnumerable()
            );

            return model;
        }

    }
}