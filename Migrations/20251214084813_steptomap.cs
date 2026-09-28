using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceAggregator.Api.Migrations
{
    /// <inheritdoc />
    public partial class steptomap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AggregateStepId1",
                table: "AggregateStepMappings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AggregateStepMappings_AggregateStepId1",
                table: "AggregateStepMappings",
                column: "AggregateStepId1");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateStepMappings_InputStepId",
                table: "AggregateStepMappings",
                column: "InputStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_AggregateStepMappings_AggregateSteps_AggregateStepId1",
                table: "AggregateStepMappings",
                column: "AggregateStepId1",
                principalTable: "AggregateSteps",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AggregateStepMappings_AggregateSteps_InputStepId",
                table: "AggregateStepMappings",
                column: "InputStepId",
                principalTable: "AggregateSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AggregateStepMappings_AggregateSteps_AggregateStepId1",
                table: "AggregateStepMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_AggregateStepMappings_AggregateSteps_InputStepId",
                table: "AggregateStepMappings");

            migrationBuilder.DropIndex(
                name: "IX_AggregateStepMappings_AggregateStepId1",
                table: "AggregateStepMappings");

            migrationBuilder.DropIndex(
                name: "IX_AggregateStepMappings_InputStepId",
                table: "AggregateStepMappings");

            migrationBuilder.DropColumn(
                name: "AggregateStepId1",
                table: "AggregateStepMappings");
        }
    }
}
