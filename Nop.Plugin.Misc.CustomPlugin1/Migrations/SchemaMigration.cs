using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.CustomPlugin1.Domains;

namespace Nop.Plugin.Misc.CustomPlugin1.Migrations;
[NopMigration("4/10/2025 12:06:42 PM", "Nop.Plugin.Misc.CustomPlugin1 schema", MigrationProcessType.Installation)]
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