using Nop.Plugin.Misc.PurchaseOrderManager.Utility;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.SubscriptionManager;
/// <summary>
/// Rename this file and change to the correct type
/// </summary>
public class SubscriptionManagerPlugin : BasePlugin
{
    private readonly ILocalizationService _localizationService;

    public SubscriptionManagerPlugin(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }
    public override async Task InstallAsync()
    {
        var resources = SubscriptionManagerLocaleResources.GetAll();

        await _localizationService.AddOrUpdateLocaleResourceAsync(resources);

        await base.InstallAsync();
    }
    public override async Task UninstallAsync()
    {
        var resourceKeys = SubscriptionManagerLocaleResources.GetAll().Keys.ToArray();

        await _localizationService.DeleteLocaleResourcesAsync(resourceKeys);

        await base.UninstallAsync();
    }
}