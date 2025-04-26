using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.PurchaseOrder;
/// <summary>
/// Rename this file and change to the correct type
/// </summary>
public class PurchaseOrderPlugin : BasePlugin
{
    public override async Task InstallAsync()
    {
        //Logic during installation goes here...

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        //Logic during uninstallation goes here...

        await base.UninstallAsync();
    }
}