using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models;
public record PurchaseOrderListModel : BasePagedListModel<PurchaseOrderModel>
{
}
