using System.Linq;
using System.Threading.Tasks;
using Nop.Plugin.Misc.Supplier.Factories;
using Nop.Plugin.Misc.Supplier.Model;
using Nop.Plugin.Misc.Supplier.Services;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.Supplier.Factories
{
    public class SupplierModelFactory : ISupplierModelFactory
    {
        private readonly ISupplierService _supplierService;

        public SupplierModelFactory(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        public async Task<SupplierListModel> PrepareSupplierListModelAsync(SupplierSearchModel searchModel)
        {
            var suppliers = await _supplierService.GetAllAsync(
                searchModel.SearchName,
                searchModel.SearchEmail,
                searchModel.Page - 1,
                searchModel.PageSize
            );

            var model = await new SupplierListModel().PrepareToGridAsync(searchModel, suppliers, () =>
            {
                return suppliers.Select(supplier => new SupplierModel
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    ContactPerson = supplier.ContactPerson,
                    Phone = supplier.Phone,
                    Email = supplier.Email,
                    Address = supplier.Address
                }).ToAsyncEnumerable();
            });

            return model;
        }
    }
}
