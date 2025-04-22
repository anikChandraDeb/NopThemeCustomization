using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Misc.Supplier.Domain;
public class ProductSupplierMapping : BaseEntity
{
    public int ProductId { get; set; }

    public int SupplierId { get; set; }
}
