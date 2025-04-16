using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Supplier.Model;
// Change the class to a record to fix CS8865
public record SupplierListModel : BasePagedListModel<SupplierModel>
{
}
