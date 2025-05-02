using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models;
using Nop.Plugin.Misc.PurchaseOrderManager.Domain;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Domain;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Services;
public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;
    private readonly IRepository<PurchaseOrderProduct> _purchaseOrderProductRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<ProductSupplierMapping> _productSupplierMapping;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PurchaseOrderService(
        IRepository<PurchaseOrder> purchaseOrderRepository,
        IRepository<PurchaseOrderProduct> purchaseOrderProductRepository,
        IRepository<Product> productRepository,
        IRepository<ProductSupplierMapping> productSupplierMapping,
        IStaticCacheManager staticCacheManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _purchaseOrderProductRepository = purchaseOrderProductRepository;
        _productRepository = productRepository;
        _staticCacheManager = staticCacheManager;
        _httpContextAccessor = httpContextAccessor;
        _productSupplierMapping = productSupplierMapping;
    }

    public async Task InsertPurchaseOrderAsync(PurchaseOrder purchaseOrder)
    {
        if (purchaseOrder == null)
            throw new ArgumentNullException(nameof(purchaseOrder));

        await _purchaseOrderRepository.InsertAsync(purchaseOrder);
    }

    public async Task UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
    {
        if (purchaseOrder == null)
            throw new ArgumentNullException(nameof(purchaseOrder));

        await _purchaseOrderRepository.UpdateAsync(purchaseOrder);
    }

    public async Task DeletePurchaseOrderAsync(PurchaseOrder purchaseOrder)
    {
        if (purchaseOrder == null)
            throw new ArgumentNullException(nameof(purchaseOrder));

        await _purchaseOrderRepository.DeleteAsync(purchaseOrder);
    }

    public async Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id)
    {
        if (id == 0)
            return null;

        return await _purchaseOrderRepository.GetByIdAsync(id);
    }

    public async Task<IList<PurchaseOrder>> GetAllPurchaseOrdersAsync()
    {
        return await _purchaseOrderRepository.Table.ToListAsync();
    }

    public async Task<IPagedList<PurchaseOrder>> SearchPurchaseOrdersAsync(
        int supplierId = 0,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageIndex = 0,
        int pageSize = int.MaxValue)
    {
        var query = _purchaseOrderRepository.Table;

        if (supplierId > 0)
            query = query.Where(po => po.SupplierId == supplierId);

        if (startDate.HasValue)
            query = query.Where(po => po.CreatedOnUtc >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(po => po.CreatedOnUtc <= endDate.Value.Date.AddDays(1));

        query = query.OrderByDescending(po => po.CreatedOnUtc);

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task InsertPurchaseOrderProductAsync(PurchaseOrderProduct product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        await _purchaseOrderProductRepository.InsertAsync(product);
    }

    public async Task UpdatePurchaseOrderProductAsync(PurchaseOrderProduct product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        await _purchaseOrderProductRepository.UpdateAsync(product);
    }

    public async Task DeletePurchaseOrderProductAsync(PurchaseOrderProduct product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        await _purchaseOrderProductRepository.DeleteAsync(product);
    }

    public async Task<PurchaseOrderProduct> GetPurchaseOrderProductByIdAsync(int id)
    {
        if (id == 0)
            return null;

        return await _purchaseOrderProductRepository.GetByIdAsync(id);
    }

    public async Task<IList<PurchaseOrderProduct>> GetProductsByPurchaseOrderIdAsync(int purchaseOrderId)
    {
        if (purchaseOrderId == 0)
            return new List<PurchaseOrderProduct>();

        return await _purchaseOrderProductRepository.Table
            .Where(p => p.PurchaseOrderId == purchaseOrderId)
            .ToListAsync();
    }

    public async Task<List<int>> GetProductIdBySupplierIdAsync(int supplierId)
    {
        // Step 1: Get all ProductIds from ProductSupplierMapping table for the given supplierId
        var productIds = await _productSupplierMapping.Table
            .Where(psm => psm.SupplierId == supplierId) // Filter by SupplierId
            .Select(psm => psm.ProductId) // Select the ProductIds
            .ToListAsync();

        return productIds;
    }
    public async Task<IList<PurchaseOrderItemModel>> GetItemsByOrderIdAsync(int orderId)
    {
        var items = await _purchaseOrderProductRepository.Table
            .Where(x => x.PurchaseOrderId == orderId)
            .ToListAsync();

        var productIds = items.Select(x => x.ProductId).Distinct().ToList();
        var products = await _productRepository.Table
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var models = items.Select(x =>
        {
            products.TryGetValue(x.ProductId, out var product);
            return new PurchaseOrderItemModel
            {
                Id = x.Id,
                PurchaseOrderId = x.PurchaseOrderId,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitCost = x.UnitCost,
                LineTotal = x.UnitCost * x.Quantity,
                ProductName = product?.Name ?? "N/A",
                Sku = product?.Sku ?? "N/A"
            };
        }).ToList();

        return models;
    }



    public async Task<IList<PurchaseOrderProduct>> GetPurchaseOrderProductsByOrderIdAsync(int orderId)
    {
        var query = from po in _purchaseOrderProductRepository.Table
                    where po.PurchaseOrderId == orderId
                    select po;
        return await query.ToListAsync();
    }
    //Product Inventory Udpate method
    public async Task UpdateProductStockQuantity(ProductModel product) {
        var productEntity = await _productRepository.GetByIdAsync(product.Id);
        if (productEntity != null)
        {
            productEntity.StockQuantity += product.StockQuantity;
            await _productRepository.UpdateAsync(productEntity);
        }
    }
    public async Task AssignProductStockQuantity(ProductModel product)
    {
        var productEntity = await _productRepository.GetByIdAsync(product.Id);
        if (productEntity != null)
        {
            productEntity.StockQuantity = product.StockQuantity;
            await _productRepository.UpdateAsync(productEntity);
        }
    }
    public async Task<ProductModel> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        var model = new ProductModel
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            StockQuantity = product.StockQuantity,
            Price = product.Price
        };
        return model;
    }
    //Session Method
    private const string TempOrderSessionKey = "TempPurchaseOrderItems";

    public Task AddTempPurchaseOrderItemAsync(PurchaseOrderItemModel item)
    {
        var items = GetSessionItems();
        var existing = items.FirstOrDefault(i => i.ProductId == item.ProductId);

        if (existing != null)
        {
            existing.Quantity += item.Quantity;
            existing.UnitCost = item.UnitCost;
            existing.LineTotal = existing.Quantity * existing.UnitCost;
        }
        else
        {
            item.LineTotal = item.Quantity * item.UnitCost;
            items.Add(item);
        }

        SaveSessionItems(items);
        return Task.CompletedTask;
    }

    public Task<List<PurchaseOrderItemModel>> GetTempPurchaseOrderItemsAsync()
    {
        var items = GetSessionItems();
        return Task.FromResult(items);
    }

    public List<PurchaseOrderItemModel> GetSessionItems()
    {
        var sessionData = _httpContextAccessor.HttpContext.Session.GetString(TempOrderSessionKey);
        if (string.IsNullOrEmpty(sessionData))
            return new List<PurchaseOrderItemModel>();

        return JsonConvert.DeserializeObject<List<PurchaseOrderItemModel>>(sessionData);
    }

    public void SaveSessionItems(List<PurchaseOrderItemModel> items)
    {
        var json = JsonConvert.SerializeObject(items);
        _httpContextAccessor.HttpContext.Session.SetString(TempOrderSessionKey, json);
    }
    public async Task ClearSessionItems()
    {
        _httpContextAccessor.HttpContext.Session.Remove(TempOrderSessionKey);
    }

}

