using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceAggregator.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConditionParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StepName",
                table: "AggregateSteps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConditionParameters",
                table: "AggregateSteps",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConditionParameters",
                table: "AggregateSteps");

            migrationBuilder.AlterColumn<string>(
                name: "StepName",
                table: "AggregateSteps",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
