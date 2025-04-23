using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Utility;
public static class SupplierLocaleResources
{
    public static Dictionary<string, string> GetAll() => new Dictionary<string, string>
    {
        // Page title
        ["Admin.Suppliers"] = "Suppliers",
        ["Admin.Suppliers.Addnew"] = "Add a new supplier",
        ["Admin.Suppliers.Info"] = "Supplier Info",
        ["Admin.Suppliers.Backtolist"] = "back to supplier list",
        ["Admin.Supplier.Added"] = "Supplier Added Successfully",
        ["Admin.Vendors.Updated"] = "Supplier Updated Successfully",
        ["Admin.Suppliers.EditSupplierDetails"] = "Edit Supplier Details",

        // Required
        ["Admin.Suppliers.Fields.Name.Required"] = "Supplier name is required",
        ["Admin.Suppliers.Fields.Email.Required"] = "Not a Valid Email",
        ["Admin.Suppliers.Fields.Phone.Required"] = "Supplier phone is required",
        ["Admin.Suppliers.Fields.Address.Required"] = "Supplier address is required",
        ["Admin.Suppliers.Fields.Description.Required"] = "Supplier description is required",
        ["Admin.Common.WrongEmail"] = "Wrong Email",
        ["Admin.Common.WrongPhone"] = "Enter Bangladeshi Phone Number",
        ["Admin.Common.ExitDescriptionLength"] = "Description length should be less than 250 characters",

        // Search fields
        ["Admin.Suppliers.List.SearchName"] = "Supplier Name",
        ["Admin.Suppliers.List.SearchName.Hint"] = "Search suppliers by their name.",
        ["Admin.Suppliers.List.SearchEmail"] = "Supplier Email",
        ["Admin.Suppliers.List.SearchEmail.Hint"] = "Search suppliers by their email address.",

        // Fields
        ["Admin.Suppliers.Fields.Name"] = "Name",
        ["Admin.Suppliers.Fields.Name.Hint"] = "Enter the supplier's name.",
        ["Admin.Suppliers.Fields.ContactPerson"] = "Contact Person",
        ["Admin.Suppliers.Fields.ContactPerson.Hint"] = "Enter the name of the supplier's main contact person.",
        ["Admin.Suppliers.Fields.Phone"] = "Phone",
        ["Admin.Suppliers.Fields.Phone.Hint"] = "Enter the supplier's contact number.",
        ["Admin.Suppliers.Fields.Email"] = "Email",
        ["Admin.Suppliers.Fields.Email.Hint"] = "Enter the supplier's email address.",
        ["Admin.Suppliers.Fields.Address"] = "Address",
        ["Admin.Suppliers.Fields.Address.Hint"] = "Enter the full address of the supplier.",
        ["Admin.Suppliers.Fields.Description"] = "Description",
        ["Admin.Suppliers.Fields.Description.Hint"] = "Enter the supplier's description",
        ["Admin.Suppliers.Fields.IsActive"] = "Active Status",
        ["Admin.Suppliers.Fields.IsActive.Hint"] = "Enter the supplier's active status",

        //Widget
        ["Admin.Supplier.Widget"]= "Supplier",
        ["Admin.Supplier.Widget.Description"]= "Suppliers are the individuals or companies that provide products to your store. For example, if you're selling electronics, you might have different suppliers for mobile phones, laptops, or accessories. Assigning a supplier to a product helps track where your inventory is sourced from and can simplify communication and reordering. You can add suppliers in Suppliers and assign them to products from the product edit page. Managing suppliers can help streamline your supply chain and maintain accurate product sourcing information.",
        ["Admin.Supplier.Widget.Message"]= "You need to save the product before you can add supplier for this page."
    };
}

