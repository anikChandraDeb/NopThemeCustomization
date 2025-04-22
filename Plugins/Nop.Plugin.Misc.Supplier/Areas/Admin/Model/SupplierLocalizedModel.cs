using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Localization;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Model;
public class SupplierLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Suppliers.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Suppliers.Fields.Address")]
    public string Address { get; set; }

    [NopResourceDisplayName("Admin.Suppliers.Fields.Description")]
    public string Description { get; set; }
}


