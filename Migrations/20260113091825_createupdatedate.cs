using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceAggregator.Api.Migrations
{
    /// <inheritdoc />
    public partial class createupdatedate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Services",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Services",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "ServiceMappings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "ServiceMappings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "AggregateSteps",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "AggregateSteps",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "AggregateStepMappings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "AggregateStepMappings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Aggregates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Aggregates",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "ServiceMappings");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "ServiceMappings");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "AggregateSteps");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "AggregateSteps");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "AggregateStepMappings");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "AggregateStepMappings");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Aggregates");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Aggregates");
        }
    }
}
