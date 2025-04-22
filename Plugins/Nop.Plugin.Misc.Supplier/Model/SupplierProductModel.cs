using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Supplier.Model;
// Change the class to a record to fix CS8865
public record SupplierProductModel
{
    public int SelectedSupplierId { get; set; }
    public List<SelectListItem> Suppliers { get; set; } = new();
}


