using Nop.Core;

namespace Nop.Plugin.Misc.Supplier.Domain
{
    public class SupplierEntity : BaseEntity
    {
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}