using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Model;
public record SupplierSearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Admin.Suppliers.List.SearchName")]
    public string SearchName { get; set; }

    [NopResourceDisplayName("Admin.Suppliers.List.SearchEmail")]
    public string SearchEmail { get; set; }
}

