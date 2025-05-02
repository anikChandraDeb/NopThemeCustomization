using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.PurchaseOrderManager.Domain;

namespace Nop.Plugin.Misc.PurchaseOrderManager.MIgration;
[NopSchemaMigration("2025/04/26 01:08:00", "Misc.PurchaseOrderManager base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<PurchaseOrder>();
        Create.TableFor<PurchaseOrderProduct>();
    }
}

