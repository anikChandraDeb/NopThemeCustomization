using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.Supplier.Areas.Admin.Domain;

namespace Nop.Plugin.Misc.Supplier.Areas.Admin.Mapping
{
    public class ProductSupplierMappingMap : NopEntityBuilder<ProductSupplierMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductSupplierMapping.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(ProductSupplierMapping.ProductId)).AsInt32().NotNullable()
                .WithColumn(nameof(ProductSupplierMapping.SupplierId)).AsInt32().Nullable();
        }
    }
}
