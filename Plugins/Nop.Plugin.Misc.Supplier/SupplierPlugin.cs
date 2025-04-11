using Microsoft.Extensions.DependencyInjection;
using Nop.Services.Events;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.Supplier;
/// <summary>
/// Rename this file and change to the correct type
/// </summary>
public class SupplierPlugin : BasePlugin
{
    private readonly IPermissionService _permissionService;

    public SupplierPlugin(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public override async Task InstallAsync()
    {
        // Installation logic here

        // Register the event consumer to add the menu item in the admin panel
        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        // Uninstallation logic here

        // You can add custom logic for removing configurations, databases, etc.

        await base.UninstallAsync();
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
                    //Url = eventMessage.GetMenuItemUrl("Supplier", "Index"), // URL for the menu item
                    Url = "/Admin/Supplier/Index", // Explicit URL for the menu item pointing to the SupplierController's Index action
                    IconClass = "far fa-dot-circle", // Icon for the menu item
                    Visible = true, // Make the menu item visible
                });
        }
    }
    
}
