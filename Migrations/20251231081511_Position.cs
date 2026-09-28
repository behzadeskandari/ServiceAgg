using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceAggregator.Api.Migrations
{
    /// <inheritdoc />
    public partial class Position : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PositionX",
                table: "AggregateSteps",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PositionY",
                table: "AggregateSteps",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionX",
                table: "AggregateSteps");

            migrationBuilder.DropColumn(
                name: "PositionY",
                table: "AggregateSteps");
        }
    }
}
