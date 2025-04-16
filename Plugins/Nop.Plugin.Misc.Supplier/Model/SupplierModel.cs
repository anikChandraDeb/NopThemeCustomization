using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Supplier.Model;
// Change SupplierModel to a record to match the inheritance requirement of BaseNopModel  
public record SupplierModel : BaseNopEntityModel
{
    
    public string Name { get; set; }
    public string ContactPerson { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
}
