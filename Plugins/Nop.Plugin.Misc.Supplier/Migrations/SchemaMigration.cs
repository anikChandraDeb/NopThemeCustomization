using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Data.Extensions;
using Nop.Plugin.Misc.Supplier.Domain;

namespace Nop.Plugin.Misc.Supplier.Migrations
{
    [NopSchemaMigration("2025/04/11 01:08:00", "Misc.Supplier base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.TableFor<SupplierEntity>();
        }
    }
}
