using Nop.Plugin.Misc.PurchaseOrderManager.Utility;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.PurchaseOrderManager;
/// <summary>
/// Rename this file and change to the correct type
/// </summary>
public class PurchaseOrderManagerPlugin : BasePlugin
{
    private readonly ILocalizationService _localizationService;

    public PurchaseOrderManagerPlugin(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }
    public override async Task InstallAsync()
    {
        var resources = PurchaseOrderLocaleResources.GetAll();

        await _localizationService.AddOrUpdateLocaleResourceAsync(resources);

        await base.InstallAsync();
    }
    public override async Task UninstallAsync()
    {
        var resourceKeys = PurchaseOrderLocaleResources.GetAll().Keys.ToArray();

        await _localizationService.DeleteLocaleResourcesAsync(resourceKeys);

        await base.UninstallAsync();
    }
}