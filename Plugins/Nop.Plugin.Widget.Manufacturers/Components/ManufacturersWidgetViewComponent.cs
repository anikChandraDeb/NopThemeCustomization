using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widget.HelloWorld.Components
{
    public class ManufacturersWidgetViewComponent : NopViewComponent
    {
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;

        public ManufacturersWidgetViewComponent(IManufacturerService manufacturerService, IPictureService pictureService)
        {
            _manufacturerService = manufacturerService;
            _pictureService = pictureService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: false);
            var pictureUrls = new List<string>();

            foreach (var manufacturer in manufacturers)
            {
                var pictureUrl = await _pictureService.GetPictureUrlAsync(manufacturer.PictureId, 200); // Optional: set width
                if (!string.IsNullOrEmpty(pictureUrl))
                    pictureUrls.Add(pictureUrl);
            }

            return View("~/Plugins/Widget.Manufacturers/Views/ManufacturesSlider.cshtml", pictureUrls);
        }
    }
}
