using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Plugin.Misc.Supplier.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;
using Nop.Services.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;

namespace Nop.Plugin.Misc.Supplier;
/// <summary>
/// Rename this file and change to the correct type
/// </summary>
public class SupplierPlugin : BasePlugin, IMiscPlugin
{
    private readonly IPermissionService _permissionService;
    private readonly ILocalizationService _localizationService;
    private readonly INopDataProvider _dataProvider;

    public SupplierPlugin(
        IPermissionService permissionService,
        ILocalizationService localizationService,
        INopDataProvider dataProvider)
    {
        _permissionService = permissionService;
        _localizationService = localizationService;
        _dataProvider = dataProvider;
    }

    public override async Task InstallAsync()
    {
        var resources = new Dictionary<string, string>
        {
            // Page title
            ["Admin.Suppliers"] = "Suppliers",
            ["Admin.Suppliers.Addnew"] = "Add a new supplier",
            ["Admin.Suppliers.Info"]= "Supplier Info",
            ["Admin.Suppliers.Backtolist"]="back to supplier list",
            ["Admin.Supplier.Added"] ="Supplier Added Successfully",
            ["Admin.Vendors.Updated"]="Supplier Updated Successfully",
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
            ["Admin.Suppliers.Fields.Address.Hint"] = "Enter the full address of the supplier."
        };

        await _localizationService.AddOrUpdateLocaleResourceAsync(resources);

        await base.InstallAsync();
    }


    public override async Task UninstallAsync()
    {
        try
        {
            // Delete localization
            var resourceKeys = new[]
            {
            "Admin.Suppliers",
            "Admin.Suppliers.List.SearchName",
            "Admin.Suppliers.List.SearchName.Hint",
            "Admin.Suppliers.List.SearchEmail",
            "Admin.Suppliers.List.SearchEmail.Hint",
            "Admin.Suppliers.Fields.Name",
            "Admin.Suppliers.Fields.Name.Hint",
            "Admin.Suppliers.Fields.ContactPerson",
            "Admin.Suppliers.Fields.ContactPerson.Hint",
            "Admin.Suppliers.Fields.Phone",
            "Admin.Suppliers.Fields.Phone.Hint",
            "Admin.Suppliers.Fields.Email",
            "Admin.Suppliers.Fields.Email.Hint",
            "Admin.Suppliers.Fields.Address",
            "Admin.Suppliers.Fields.Address.Hint"
        };
            await _localizationService.DeleteLocaleResourcesAsync(resourceKeys);

            // Drop table safely
            await _dataProvider.ExecuteNonQueryAsync("DROP TABLE IF EXISTS [Supplier]");

            // Complete base uninstall
            await base.UninstallAsync();
        }
        catch (Exception ex)
        {
            // Optional: log the exception
            // _logger.InsertLog(LogLevel.Error, "Error uninstalling SupplierPlugin", ex.Message, ex);
            throw; // Re-throw to keep uninstall logic clean
        }
    }






    // Add an event consumer to add a menu item in the admin panel
    public class EventConsumer : IConsumer<AdminMenuCreatedEvent>
    {
        private readonly IPermissionService _permissionService;

        public EventConsumer(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
        {
            // Check for permissions before adding the menu item
            if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PLUGINS))
                return;

            // Add custom menu item in the admin panel
            eventMessage.RootMenuItem.InsertBefore("Local plugins",
                new AdminMenuItem
                {
                    SystemName = "Misc.Supplier", // Unique name for your plugin
                    Title = "Supplier", // Title for the menu item
                    Url = eventMessage.GetMenuItemUrl("Supplier", "Index"), // URL for the menu item
                    //Url = "/Admin/Supplier/Index", // Explicit URL for the menu item pointing to the SupplierController's Index action
                    IconClass = "far fa-dot-circle", // Icon for the menu item
                    Visible = true, // Make the menu item visible
                });
        }
    }
    
}
