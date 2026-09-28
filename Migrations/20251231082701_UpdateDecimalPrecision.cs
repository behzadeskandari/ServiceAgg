using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceAggregator.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PositionY",
                table: "AggregateSteps",
                type: "decimal(18,13)",
                precision: 18,
                scale: 13,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PositionX",
                table: "AggregateSteps",
                type: "decimal(18,13)",
                precision: 18,
                scale: 13,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PositionY",
                table: "AggregateSteps",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,13)",
                oldPrecision: 18,
                oldScale: 13,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PositionX",
                table: "AggregateSteps",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,13)",
                oldPrecision: 18,
                oldScale: 13,
                oldNullable: true);
        }
    }
}
