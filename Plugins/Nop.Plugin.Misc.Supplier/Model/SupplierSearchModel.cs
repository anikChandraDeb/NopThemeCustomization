using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Supplier.Model;
// Change the class to a record to fix CS8865
public record SupplierSearchModel : BaseSearchModel
{
    public string SearchName { get; set; }
    public string SearchEmail { get; set; }

    public IPagedList<SupplierModel> Suppliers { get; set; }
}
