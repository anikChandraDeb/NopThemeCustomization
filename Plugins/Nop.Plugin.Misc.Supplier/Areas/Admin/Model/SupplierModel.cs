    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Model;
public record SupplierModel : BaseNopEntityModel, ILocalizedModel<SupplierLocalizedModel>
{
    [NopResourceDisplayName("Admin.Suppliers.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Suppliers.Fields.ContactPerson")]
    public string ContactPerson { get; set; }

    [NopResourceDisplayName("Admin.Suppliers.Fields.Phone")]
    public string Phone { get; set; }

    [NopResourceDisplayName("Admin.Suppliers.Fields.Email")]
    public string Email { get; set; }

    [NopResourceDisplayName("Admin.Suppliers.Fields.Address")]
    public string Address { get; set; }

    [NopResourceDisplayName("Admin.Suppliers.Fields.Description")]
    public string Description { get; set; } 

    [NopResourceDisplayName("Admin.Suppliers.Fields.IsActive")]
    public bool IsActive { get; set; } 

    public IList<SupplierLocalizedModel> Locales { get; set; }

    public SupplierModel()
    {
        Locales = new List<SupplierLocalizedModel>();
    }
}

