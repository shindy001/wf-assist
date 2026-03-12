using FluentMigrator;

namespace WfAssist.AspNetCore.Modules.Workflows.Infrastructure.Migrations;

[Migration(20251009_01)]
public class InitializeDb_20251009_01 : Migration
{
    public override void Up()
    {
        Create.Table("Workflows")
            .WithColumn("Id").AsFixedLengthString(36).PrimaryKey() // uuid
            .WithColumn("Name").AsFixedLengthString(100)
            .WithColumn("Data").AsString();

        Create.Table("Executions")
            .WithColumn("Id").AsFixedLengthString(36).PrimaryKey() // uuid
            .WithColumn("Status").AsFixedLengthString(100)
            .WithColumn("Snapshot").AsString()
            .WithColumn("ProcessingResults").AsString();
    }

    public override void Down()
    {
        Delete.Table("Workflows");
        Delete.Table("Executions");
    }
}