using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Domain;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Services;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Services;
using Nop.Services.Customers;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Factories
{
    public class PurchaseOrderModelFactory : IPurchaseOrderModelFactory
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly ISupplierService _supplierService;
        private readonly ICustomerService _customerService;
        private readonly IStaticCacheManager _staticCacheManager;

        public PurchaseOrderModelFactory(
            IPurchaseOrderService purchaseOrderService,
            ISupplierService supplierService,
            ICustomerService customerService,
            IStaticCacheManager staticCacheManager)
        {
            _purchaseOrderService = purchaseOrderService;
            _supplierService = supplierService;
            _customerService = customerService;
            _staticCacheManager = staticCacheManager;
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
                        CreatedOnUtc = po.CreatedOnUtc,
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
                model.CreatedOnUtc = purchaseOrder.CreatedOnUtc;
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
    }
}