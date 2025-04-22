using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Model;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Validators;

public partial class AddressValidator : BaseNopValidator<SupplierModel>
{
    public AddressValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Suppliers.Fields.Name.Required"));
        RuleFor(x => x.ContactPerson).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Suppliers.Fields.ContactPerson.Required"));
        RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Suppliers.Fields.Email.Required"))
            .IsEmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
        RuleFor(x => x.Phone).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Suppliers.Fields.Phone.Required"))
            .Matches(@"^(?:\+88|88)?01[3-9]\d{8}$")
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongPhone"));
        RuleFor(x => x.Address).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Suppliers.Fields.Address.Required"));
        RuleFor(x => x.Description).MaximumLength(250).WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.ExitDescriptionLength"));
    }
}
