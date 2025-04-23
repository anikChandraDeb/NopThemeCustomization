using Nop.Core.Caching;

namespace Nop.Plugin.Misc.Supplier;
public static class SupplierDefaults
{
    public static CacheKey AdminSupplierAllModelKey => new("Nop.supplier.admin.model-{0}-{1}-{2}-{3}", AdminSupplierAllPrefixCacheKey);
    public static string AdminSupplierAllPrefixCacheKey => "Nop.supplier.admin.model";
}

