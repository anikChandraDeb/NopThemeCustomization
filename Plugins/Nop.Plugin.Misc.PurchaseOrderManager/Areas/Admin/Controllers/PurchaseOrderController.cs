using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Nop.Core;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Factories;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models;
using Nop.Plugin.Misc.PurchaseOrderManager.Domain;
using Nop.Plugin.Misc.PurchaseOrderManager.Services; 
using Nop.Plugin.Misc.Supplier.Areas.Admin.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Controllers;
[AuthorizeAdmin]
[Area("admin")]
public class PurchaseOrderController : BasePluginController
{
    private readonly IPurchaseOrderModelFactory _purchaseOrderModelFactory;
    private readonly IPermissionService _permissionService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IPurchaseOrderService _purchaseOrderService;
    private readonly IWorkContext _workContext;
    private readonly ISupplierService _supplierService;
    private readonly ICustomerService _customerService;

    public PurchaseOrderController(
        IPurchaseOrderModelFactory purchaseOrderModelFactory,
        IPermissionService permissionService,
        INotificationService notificationService,
        ILocalizationService localizationService,
        IProductService productService,
        ICategoryService categoryService,
        IPurchaseOrderService purchaseOrderService,
        IWorkContext workContext,
        ISupplierService supplierService,
        ICustomerService customerService
        )
    {
        _purchaseOrderModelFactory = purchaseOrderModelFactory;
        _permissionService = permissionService;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _productService = productService;
        _categoryService = categoryService;
        _purchaseOrderService = purchaseOrderService;
        _workContext = workContext;
        _supplierService = supplierService;
        _customerService = customerService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _purchaseOrderModelFactory.PreparePurchaseOrderSearchModelAsync(new PurchaseOrderSearchModel());
        return View("~/Plugins/Nop.Plugin.Misc.PurchaseOrderManager/Areas/Admin/Views/PurchaseOrder/Index.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> List(PurchaseOrderSearchModel searchModel)
    {
        var model = await _purchaseOrderModelFactory.PreparePurchaseOrderListModelAsync(searchModel);
        return Json(model);
    }

    public async Task<IActionResult> Create()
    {
        var model = await _purchaseOrderModelFactory.PreparePurchaseOrderWithSuppliersModelAsync();

        return View("~/Plugins/Nop.Plugin.Misc.PurchaseOrderManager/Areas/Admin/Views/PurchaseOrder/Create.cshtml", model);
    }



    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public async Task<IActionResult> Create(PurchaseOrderModel model, bool continueEditing)
    {

        if (!ModelState.IsValid)
        {
            return View("~/Plugins/Nop.Plugin.Misc.PurchaseOrderManager/Areas/Admin/Views/PurchaseOrder/Create.cshtml", model);
        }

        var purchaseOrder = await _purchaseOrderModelFactory.PreparePurchaseOrderAsync(model);
        await _purchaseOrderService.InsertPurchaseOrderAsync(purchaseOrder);

        var orderItems = _purchaseOrderModelFactory.PrepareOrderItems(purchaseOrder.Id, model.Items);
        foreach (var item in orderItems)
        {
            await _purchaseOrderService.InsertPurchaseOrderProductAsync(item);

            await _purchaseOrderService.UpdateProductStockQuantity(new ProductModel
            {
                Id = item.ProductId,
                StockQuantity = item.Quantity
            });
        }

        await _purchaseOrderService.ClearSessionItems();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.PurchaseOrders.Added"));
        return continueEditing ? RedirectToAction("Edit", new { id = purchaseOrder.Id }) : RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _purchaseOrderModelFactory.PreparePurchaseOrderModelAsync(id);
        if (model == null)
            return RedirectToAction("List");

        return View("~/Plugins/Nop.Plugin.Misc.PurchaseOrderManager/Areas/Admin/Views/PurchaseOrder/Edit.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public async Task<IActionResult> Edit(PurchaseOrderModel model, bool continueEditing)
    {   
        if (ModelState.IsValid)
        {
            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(model.Id);
            if (purchaseOrder == null)
                return RedirectToAction("Index");

            purchaseOrder.SupplierId = model.SupplierId;
            purchaseOrder.CreatedOnUtc = model.OrderDate;
            purchaseOrder.TotalAmount = model.TotalAmount;

            await _purchaseOrderService.UpdatePurchaseOrderAsync(purchaseOrder);

            var existingItems = await _purchaseOrderService.GetProductsByPurchaseOrderIdAsync(purchaseOrder.Id);
            var submittedItemIds = model.Items.Select(i => i.Id).ToList();

            foreach (var item in model.Items)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                if (item.Id > 0)
                {
                    var existing = existingItems.FirstOrDefault(x => x.Id == item.Id);
                    if (existing != null)
                    {
                        if (product != null)
                            product.StockQuantity = product.StockQuantity - existing.Quantity;
                        existing.ProductId = item.ProductId;
                        existing.Quantity = item.Quantity;
                        existing.UnitCost = item.UnitCost;
                        existing.LineTotal = item.Quantity * item.UnitCost;
                        await _purchaseOrderService.UpdatePurchaseOrderProductAsync(existing);
                    }
                }
                else
                {
                    var newItem = new PurchaseOrderProduct
                    {
                        PurchaseOrderId = purchaseOrder.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitCost = item.UnitCost,
                        LineTotal = item.LineTotal
                    };
                    await _purchaseOrderService.InsertPurchaseOrderProductAsync(newItem);
                }
                if (product != null)
                {
                    var productModel = new ProductModel
                    {
                        Id = item.ProductId,
                        StockQuantity = product.StockQuantity + item.Quantity
                    };
                    await _purchaseOrderService.AssignProductStockQuantity(productModel);
                }
            }

            var removedItems = existingItems.Where(x => !submittedItemIds.Contains(x.Id)).ToList();
            foreach (var removed in removedItems)
            {
                await _purchaseOrderService.DeletePurchaseOrderProductAsync(removed);
            }

            await _purchaseOrderService.ClearSessionItems();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.PurchaseOrders.Updated"));
            return continueEditing ? RedirectToAction("Edit", new { id = purchaseOrder.Id }) : RedirectToAction("Index");
        }

        return View("~/Plugins/Nop.Plugin.Misc.PurchaseOrderManager/Areas/Admin/Views/PurchaseOrder/Edit.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int Id)
    {
        var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(Id);

        if (purchaseOrder == null)
            return RedirectToAction("Index");

        var purchaseOrderProducts = await _purchaseOrderService.GetProductsByPurchaseOrderIdAsync(Id);

        foreach (var product in purchaseOrderProducts)
        {
            await _purchaseOrderService.DeletePurchaseOrderProductAsync(product);
        }

        await _purchaseOrderService.DeletePurchaseOrderAsync(purchaseOrder);
        _notificationService.SuccessNotification("Purchase Order Deleted Successfully..");

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> AddProductPopup(int supplierId, string btnId, string formId)
    {
        var model = await _purchaseOrderModelFactory.PrepareAddProductPopupModelAsync(supplierId);

        ViewBag.btnId = btnId;
        ViewBag.formId = formId;

        return View("~/Plugins/Nop.Plugin.Misc.PurchaseOrderManager/Areas/Admin/Views/PurchaseOrder/AddProductPopup.cshtml", model);
    }


    [HttpPost]
    public async Task<IActionResult> ProductListForPurchaseOrder(AddProductToPurchaseOrderSearchModel searchModel)
    {

        var model = await _purchaseOrderModelFactory.PrepareSupplierProductListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddProductsToOrder([FromBody] AddProductsRequest model)
    {
        if (model?.SelectedIds == null)
            return BadRequest("Invalid data");

        var items = await _purchaseOrderModelFactory.PrepareTempOrderItemsAsync(model);

        foreach (var item in items)
        {
            await _purchaseOrderService.AddTempPurchaseOrderItemAsync(item);
        }

        return Ok();
    }


    public async Task<IActionResult> GetOrderItems()
    {
        var items = _purchaseOrderService.GetSessionItems(); 

        return Json(items);
    }

    [HttpGet]
    public IActionResult GetPurchaseOrderItems()
    {
        var model = new PurchaseOrderModel();

        model.Items = _purchaseOrderService.GetSessionItems(); 
        model.TotalAmount = model.Items.Sum(item => item.LineTotal);
        return PartialView("~/Plugins/Nop.Plugin.Misc.PurchaseOrderManager/Areas/Admin/Views/PurchaseOrder/_PurchaseOrderItems.cshtml", model);
    }

    [HttpGet]
    public IActionResult ViewTempPurchaseOrderItems()
        {
        var items = _purchaseOrderService.GetSessionItems(); 
        return Ok(new DataTablesModel { Data = items });
        //return Json(items);
    }


    [HttpPost]
    public async Task<IActionResult> DeletePurchaseOrderItem(int productId)
    {
        var items = _purchaseOrderService.GetSessionItems();
        await _purchaseOrderService.ClearSessionItems();
        var itemToRemove = items.FirstOrDefault(x => x.ProductId == productId);
        if (itemToRemove != null)
        {
            items.Remove(itemToRemove);
            _purchaseOrderService.SaveSessionItems(items);
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult UpdatePurchaseOrderItem(int productId, int quantity, decimal unitCost)
    {
        var items = _purchaseOrderService.GetSessionItems();
        var item = items?.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            item.Quantity = quantity;
            item.UnitCost = unitCost;
            item.LineTotal = quantity * unitCost;
        }
        _purchaseOrderService.SaveSessionItems(items);
        return Json(new { success = true });

    }

    [HttpPost]
    public async Task<IActionResult> ClearSession()
    {
        await _purchaseOrderService.ClearSessionItems(); 
        return Ok();
    }
}
