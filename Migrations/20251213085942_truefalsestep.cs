using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceAggregator.Api.Migrations
{
    /// <inheritdoc />
    public partial class truefalsestep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AggregateStepMappings_AggregateSteps_InputStepId",
                table: "AggregateStepMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_AggregateSteps_AggregateSteps_SourceStepId",
                table: "AggregateSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_AggregateSteps_Aggregates_AggregateId",
                table: "AggregateSteps");

            migrationBuilder.DropIndex(
                name: "IX_AggregateStepMappings_InputStepId",
                table: "AggregateStepMappings");

            migrationBuilder.RenameColumn(
                name: "SourceStepId",
                table: "AggregateSteps",
                newName: "TrueStepId");

            migrationBuilder.RenameIndex(
                name: "IX_AggregateSteps_SourceStepId",
                table: "AggregateSteps",
                newName: "IX_AggregateSteps_TrueStepId");

            migrationBuilder.AddColumn<Guid>(
                name: "FalseStepId",
                table: "AggregateSteps",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AggregateSteps_FalseStepId",
                table: "AggregateSteps",
                column: "FalseStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_AggregateSteps_AggregateSteps_FalseStepId",
                table: "AggregateSteps",
                column: "FalseStepId",
                principalTable: "AggregateSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AggregateSteps_AggregateSteps_TrueStepId",
                table: "AggregateSteps",
                column: "TrueStepId",
                principalTable: "AggregateSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AggregateSteps_Aggregates_AggregateId",
                table: "AggregateSteps",
                column: "AggregateId",
                principalTable: "Aggregates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AggregateSteps_AggregateSteps_FalseStepId",
                table: "AggregateSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_AggregateSteps_AggregateSteps_TrueStepId",
                table: "AggregateSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_AggregateSteps_Aggregates_AggregateId",
                table: "AggregateSteps");

            migrationBuilder.DropIndex(
                name: "IX_AggregateSteps_FalseStepId",
                table: "AggregateSteps");

            migrationBuilder.DropColumn(
                name: "FalseStepId",
                table: "AggregateSteps");

            migrationBuilder.RenameColumn(
                name: "TrueStepId",
                table: "AggregateSteps",
                newName: "SourceStepId");

            migrationBuilder.RenameIndex(
                name: "IX_AggregateSteps_TrueStepId",
                table: "AggregateSteps",
                newName: "IX_AggregateSteps_SourceStepId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateStepMappings_InputStepId",
                table: "AggregateStepMappings",
                column: "InputStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_AggregateStepMappings_AggregateSteps_InputStepId",
                table: "AggregateStepMappings",
                column: "InputStepId",
                principalTable: "AggregateSteps",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AggregateSteps_AggregateSteps_SourceStepId",
                table: "AggregateSteps",
                column: "SourceStepId",
                principalTable: "AggregateSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AggregateSteps_Aggregates_AggregateId",
                table: "AggregateSteps",
                column: "AggregateId",
                principalTable: "Aggregates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
