using Nop.Core;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Domain;
public class ProductSupplierMapping : BaseEntity
{
    public int ProductId { get; set; }

    public int SupplierId { get; set; }
}
