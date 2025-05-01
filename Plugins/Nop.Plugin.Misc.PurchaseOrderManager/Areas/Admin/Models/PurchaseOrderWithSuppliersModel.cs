using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.PurchaseOrderManager.Models;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models;
public record PurchaseOrderWithSuppliersModel : PurchaseOrderModel
{
    // List of available suppliers to be used in the view
    public IList<SelectListItem> AvailableSuppliers { get; set; }
}
