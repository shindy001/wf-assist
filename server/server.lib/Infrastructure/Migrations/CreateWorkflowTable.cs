using FluentMigrator;

namespace WfAssist.AspNetCore.Infrastructure.Migrations;

[Migration(2025100901)]
public class CreateWorkflowTable : Migration
{
    public override void Up()
    {
        Create.Table("Workflows")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity();
    }

    public override void Down()
    {
        Delete.Table("Workflows");
    }
}