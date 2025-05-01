using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Models;
public class AddProductsRequest
{
    public List<int> SelectedIds { get; set; }
    public Dictionary<int, decimal> Prices { get; set; }
    public Dictionary<int, int> Quantities { get; set; }
    public int SupplierId { get; set; }
}
