using FluentMigrator;

namespace WfAssist.AspNetCore.Infrastructure.Migrations;

[Migration(2025100901)]
public class CreateWorkflowTable : Migration
{
    public override void Up()
    {
        Create.Table("Workflows")
            .WithColumn("Id").AsFixedLengthString(36).PrimaryKey() // uuid
            .WithColumn("Name").AsFixedLengthString(100)
            .WithColumn("Data").AsString();
    }

    public override void Down()
    {
        Delete.Table("Workflows");
    }
}