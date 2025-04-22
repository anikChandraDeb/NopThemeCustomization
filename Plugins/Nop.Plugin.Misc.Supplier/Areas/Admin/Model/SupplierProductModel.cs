using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Model;
public record SupplierProductModel
{
    public int ProductId { get; set; }

    public int SelectedSupplierId { get; set; }

    public string SelectedSupplierName { get; set; }
    public IList<Domain.SupplierEntity> Suppliers { get; set; }
}


