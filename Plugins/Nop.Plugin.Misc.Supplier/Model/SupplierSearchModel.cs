using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Supplier.Model;
// Change the class to a record to fix CS8865
public record SupplierSearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Admin.Suppliers.List.SearchName")]
    public string SearchName { get; set; }

    [NopResourceDisplayName("Admin.Suppliers.List.SearchEmail")]
    public string SearchEmail { get; set; }
}

