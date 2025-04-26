using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Domain;

namespace Nop.Plugin.Misc.PurchaseOrderManager.Areas.Admin.Mapping;
public class PurchaseOrderProductBuilder : NopEntityBuilder<PurchaseOrderProduct>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(PurchaseOrderProduct.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(PurchaseOrderProduct.PurchaseOrderId)).AsInt32().NotNullable()
            .WithColumn(nameof(PurchaseOrderProduct.ProductId)).AsInt32().NotNullable()
            .WithColumn(nameof(PurchaseOrderProduct.Quantity)).AsInt32().NotNullable()
            .WithColumn(nameof(PurchaseOrderProduct.UnitCost)).AsDecimal(18, 4).NotNullable()
            .WithColumn(nameof(PurchaseOrderProduct.LineTotal)).AsDecimal(18, 4).NotNullable();
    }
}