using Nop.Data;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Components;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Utility;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.Supplier;
public class SupplierPlugin : BasePlugin, IMiscPlugin , IWidgetPlugin
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
        var resources = SupplierLocaleResources.GetAll();

        await _localizationService.AddOrUpdateLocaleResourceAsync(resources);

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        try
        {
            var resourceKeys = SupplierLocaleResources.GetAll().Keys.ToArray();

            await _localizationService.DeleteLocaleResourcesAsync(resourceKeys);

            await _dataProvider.ExecuteNonQueryAsync("DROP TABLE IF EXISTS [Supplier]");

            await base.UninstallAsync();
        }
        catch (Exception ex)
        {
            // Optional: log the exception
            // _logger.InsertLog(LogLevel.Error, "Error uninstalling SupplierPlugin", ex.Message, ex);
            throw; // Re-throw to keep uninstall logic clean
        }
    }



    public override async Task UpdateAsync(string currentVersion, string targetVersion)
    {
        var current = Version.Parse(currentVersion);
        var target = Version.Parse(targetVersion);

        if (current < target)
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Admin.Suppliers.Fields.ContactPerson.Required"] = "Contact Person is Required!"
            });
        }
    }

    public bool HideInWidgetList => false;

    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(SupplierWidgetViewComponent);
    }

    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>
        {
            AdminWidgetZones.ProductDetailsBlock
        });
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
            if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PLUGINS))
                return;

            eventMessage.RootMenuItem.InsertAfter("Catalog",new AdminMenuItem
            {
                SystemName = "Misc.Supplier",
                Title = "Supplier",
                Url = eventMessage.GetMenuItemUrl("Supplier", "Index"),
                IconClass = "far fa-dot-circle",
                Visible = true
            });
        }
    }
    
}
