using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.SubscriptionManager.Domains;

namespace Nop.Plugin.Misc.SubscriptionManager.Migrations;
[NopMigration("5/6/2025 8:07:55 PM", "Nop.Plugin.Misc.SubscriptionManager schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        Create.TableFor<CustomTable>();
    }
}