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

        if (ModelState.IsValid)
        {
            var purchaseOrder = new PurchaseOrder
            {
                SupplierId = model.SupplierId,
                CreatedOnUtc = model.OrderDate,
                TotalAmount = model.TotalAmount,
                CreatedById = (await _workContext.GetCurrentCustomerAsync()).Id
            };
            await _purchaseOrderService.InsertPurchaseOrderAsync(purchaseOrder);

            //model.Items = _purchaseOrderService.GetSessionItems();

             foreach (var item in model.Items)
             {
                var orderItem = new PurchaseOrderProduct
                {
                    PurchaseOrderId = purchaseOrder.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    LineTotal = item.LineTotal
                };
                await _purchaseOrderService.InsertPurchaseOrderProductAsync(orderItem);
                var productModel= new ProductModel
                {
                    Id = item.ProductId,
                    StockQuantity = item.Quantity
                };
                await _purchaseOrderService.UpdateProductStockQuantity(productModel);
            }
            await _purchaseOrderService.ClearSessionItems();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.PurchaseOrders.Added"));
            return continueEditing ? RedirectToAction("Edit", new { id = purchaseOrder.Id }) : RedirectToAction("Index");
        }

        // If we got this far, something failed, redisplay form
        return View("~/Plugins/Nop.Plugin.Misc.PurchaseOrderManager/Areas/Admin/Views/PurchaseOrder/Create.cshtml", model);
    }
    public async Task<IActionResult> AddProductPopup(int supplierId, string btnId, string formId)
    {
        var model = new AddProductToPurchaseOrderSearchModel
        {
            SupplierId = supplierId,
            SelectedProductIds = new List<int>(),
            AvailablePageSizes = "10, 15, 20, 50, 100"
        };

        // Populate available categories
        var categories = await _categoryService.GetAllCategoriesAsync(showHidden: true);
        foreach (var category in categories)
        {
            model.AvailableCategories.Add(new SelectListItem
            {
                Text = category.Name,
                Value = category.Id.ToString()
            });
        }

        // Insert "All" at the top
        model.AvailableCategories.Insert(0, new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.Common.All"),
            Value = "0"
        });

        ViewBag.btnId = btnId;
        ViewBag.formId = formId;

        return View("~/Plugins/Nop.Plugin.Misc.PurchaseOrderManager/Areas/Admin/Views/PurchaseOrder/AddProductPopup.cshtml", model);
    }


    [HttpPost]
    public async Task<IActionResult> ProductListForPurchaseOrder(AddProductToPurchaseOrderSearchModel searchModel)
    {

        // Prepare the model using your custom factory method
        var model = await _purchaseOrderModelFactory.PrepareSupplierProductListModelAsync(searchModel);

        // Return the data in JSON format for DataTables
        return Json(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddProductsToOrder([FromBody] AddProductsRequest model)
    {
        if (model == null || model.SelectedIds == null)
            return BadRequest("Invalid data");

        foreach (var productId in model.SelectedIds)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                continue;

            var quantity = model.Quantities != null && model.Quantities.ContainsKey(productId)
                ? model.Quantities[productId]
                : 1;

            var unitCost = model.Prices != null && model.Prices.ContainsKey(productId)
                ? model.Prices[productId]
                : 0;

            await _purchaseOrderService.AddTempPurchaseOrderItemAsync(new PurchaseOrderItemModel
            {
                ProductId = productId,
                ProductName = product.Name,
                Sku = product.Sku,
                Quantity = quantity,
                UnitCost = unitCost,
                LineTotal = quantity * unitCost
            });
        }

        return Ok();
    }


    public async Task<IActionResult> GetOrderItems()
    {
        var items = _purchaseOrderService.GetSessionItems(); // List<PurchaseOrderItemModel>

        return Json(items);
    }

    [HttpGet]
    public IActionResult GetPurchaseOrderItems()
    {
        var model = new PurchaseOrderModel();
        // Load data from session or wherever you store the temporary order

        model.Items = _purchaseOrderService.GetSessionItems(); // Adjust as needed
        model.TotalAmount = model.Items.Sum(item => item.LineTotal);
        return PartialView("~/Plugins/Nop.Plugin.Misc.PurchaseOrderManager/Areas/Admin/Views/PurchaseOrder/_PurchaseOrderItems.cshtml", model);
    }

        [HttpGet]
        public IActionResult ViewTempPurchaseOrderItems()
            {
            var items = _purchaseOrderService.GetSessionItems(); // make this method public for test
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
            // Use the service to save updated items to session
            // Since SaveSessionItems is private, add a new public method in the service
            _purchaseOrderService.SaveSessionItems(items);
        }

        return Json(new { success = true });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);
        if (purchaseOrder == null)
            return RedirectToAction("List");
        var suppliers = await _supplierService.GetAllSuppliersAsync();
        var supplier = suppliers.FirstOrDefault(s => s.Id == purchaseOrder.SupplierId);

        var purchaseOrderProducts = await _purchaseOrderService.GetItemsByOrderIdAsync(purchaseOrder.Id);
        var createdBy = await _customerService.GetCustomerByIdAsync(purchaseOrder.CreatedById);
        var model = new PurchaseOrderModel
        {
            Id = purchaseOrder.Id,
            SupplierId = purchaseOrder.SupplierId,
            SupplierName=supplier.Name,
            OrderDate = purchaseOrder.CreatedOnUtc,
            CreatedById=purchaseOrder.CreatedById,
            CreatedBy= createdBy?.Email ?? "System",
            // other properties
            AvailableSuppliers = suppliers.Select(supplier => new SelectListItem
            {
                Value = supplier.Id.ToString(),
                Text = supplier.Name
            }).ToList(),
            Items = purchaseOrderProducts
        };
        _purchaseOrderService.SaveSessionItems((List<PurchaseOrderItemModel>)purchaseOrderProducts);
        var item = _purchaseOrderService.GetSessionItems();
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

            // Update purchase order main fields
            purchaseOrder.SupplierId = model.SupplierId;
            purchaseOrder.CreatedOnUtc = model.OrderDate;
            purchaseOrder.TotalAmount = model.TotalAmount;

            await _purchaseOrderService.UpdatePurchaseOrderAsync(purchaseOrder);

            // Load existing items
            var existingItems = await _purchaseOrderService.GetProductsByPurchaseOrderIdAsync(purchaseOrder.Id);
            var submittedItemIds = model.Items.Select(i => i.Id).ToList();

            // Update or add items
            foreach (var item in model.Items)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                if (item.Id > 0)
                {
                    // Update existing
                    var existing = existingItems.FirstOrDefault(x => x.Id == item.Id);
                    if (existing != null)
                    {
                        if(product!=null) product.StockQuantity = product.StockQuantity - existing.Quantity;
                        existing.ProductId = item.ProductId;
                        existing.Quantity = item.Quantity;
                        existing.UnitCost = item.UnitCost;
                        existing.LineTotal = item.Quantity*item.UnitCost;
                        await _purchaseOrderService.UpdatePurchaseOrderProductAsync(existing);
                    }
                }
                else
                {
                    // Add new
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
                        StockQuantity = product.StockQuantity+ item.Quantity
                    };
                    await _purchaseOrderService.AssignProductStockQuantity(productModel);
                }
            }

            // Delete removed items
            var removedItems = existingItems.Where(x => !submittedItemIds.Contains(x.Id)).ToList();
            foreach (var removed in removedItems)
            {
                await _purchaseOrderService.DeletePurchaseOrderProductAsync(removed);
            }

            // Optionally clear session (if using it in edit too)
            await _purchaseOrderService.ClearSessionItems();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.PurchaseOrders.Updated"));
            return continueEditing ? RedirectToAction("Edit", new { id = purchaseOrder.Id }) : RedirectToAction("Index");
        }

        return View("~/Plugins/Nop.Plugin.Misc.PurchaseOrderManager/Areas/Admin/Views/PurchaseOrder/Edit.cshtml", model);
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
    public async Task<IActionResult> Delete(int Id)
    {
        // Ensure the purchase order exists
        var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(Id);

        if (purchaseOrder == null)
            return RedirectToAction("Index");
        // Get all products associated with the purchase order
        var purchaseOrderProducts = await _purchaseOrderService.GetProductsByPurchaseOrderIdAsync(Id);

        // Delete all products
        foreach (var product in purchaseOrderProducts)
        {
            await _purchaseOrderService.DeletePurchaseOrderProductAsync(product);
        }

        await _purchaseOrderService.DeletePurchaseOrderAsync(purchaseOrder);
        _notificationService.SuccessNotification("Purchase Order Deleted Successfully..");

        // Redirect back to the purchase order edit page
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ClearSession()
    {
        await _purchaseOrderService.ClearSessionItems(); // This calls your service method
        return Ok();
    }


}
