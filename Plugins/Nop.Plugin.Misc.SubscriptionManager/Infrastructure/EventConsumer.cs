using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Infrastructure;
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

        eventMessage.RootMenuItem.InsertAfter("Catalog", new AdminMenuItem
        {
            SystemName = "Misc.SubscriptionManager",
            Title = "Subscription",
            Url = eventMessage.GetMenuItemUrl("Subscription", "Index"),
            IconClass = "far fa-dot-circle",
            Visible = true
        });
    }
}
