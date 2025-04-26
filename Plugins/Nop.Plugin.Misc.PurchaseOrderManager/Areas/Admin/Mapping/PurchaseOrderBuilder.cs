using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Domain;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Mapping;
public class PurchaseOrderBuilder : NopEntityBuilder<PurchaseOrder>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(PurchaseOrder.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(PurchaseOrder.SupplierId)).AsInt32().NotNullable()
            .WithColumn(nameof(PurchaseOrder.CreatedOnUtc)).AsDateTime().NotNullable()
            .WithColumn(nameof(PurchaseOrder.TotalAmount)).AsDecimal(18, 4).NotNullable()
            .WithColumn(nameof(PurchaseOrder.CreatedById)).AsInt32().NotNullable();
    }
}
