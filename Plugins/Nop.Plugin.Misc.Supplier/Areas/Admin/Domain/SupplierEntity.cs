using Nop.Core;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Domain
{
    public class SupplierEntity : BaseEntity, ILocalizedEntity
    {
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Description { get; set; } 
        public bool IsActive { get; set; }      
    }

}