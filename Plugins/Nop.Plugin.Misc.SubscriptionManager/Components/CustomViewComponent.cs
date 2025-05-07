using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.SubscriptionManager.Components;
[ViewComponent(Name = "Custom")]
public class CustomViewComponent : NopViewComponent
{
    public CustomViewComponent()
    {

    }

    public IViewComponentResult Invoke(int productId)
    {
        throw new NotImplementedException();
    }
}
