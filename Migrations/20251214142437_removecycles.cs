using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceAggregator.Api.Migrations
{
    /// <inheritdoc />
    public partial class removecycles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AggregateStepMappings_AggregateSteps_InputStepId",
                table: "AggregateStepMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_AggregateSteps_AggregateSteps_FalseStepId",
                table: "AggregateSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_AggregateSteps_AggregateSteps_NextStepId",
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

            migrationBuilder.DropIndex(
                name: "IX_AggregateSteps_NextStepId",
                table: "AggregateSteps");

            migrationBuilder.DropIndex(
                name: "IX_AggregateSteps_TrueStepId",
                table: "AggregateSteps");

            migrationBuilder.DropIndex(
                name: "IX_AggregateStepMappings_InputStepId",
                table: "AggregateStepMappings");

            migrationBuilder.AddForeignKey(
                name: "FK_AggregateSteps_Aggregates_AggregateId",
                table: "AggregateSteps",
                column: "AggregateId",
                principalTable: "Aggregates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AggregateSteps_Aggregates_AggregateId",
                table: "AggregateSteps");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateSteps_FalseStepId",
                table: "AggregateSteps",
                column: "FalseStepId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateSteps_NextStepId",
                table: "AggregateSteps",
                column: "NextStepId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateSteps_TrueStepId",
                table: "AggregateSteps",
                column: "TrueStepId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateStepMappings_InputStepId",
                table: "AggregateStepMappings",
                column: "InputStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_AggregateStepMappings_AggregateSteps_InputStepId",
                table: "AggregateStepMappings",
                column: "InputStepId",
                principalTable: "AggregateSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AggregateSteps_AggregateSteps_FalseStepId",
                table: "AggregateSteps",
                column: "FalseStepId",
                principalTable: "AggregateSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AggregateSteps_AggregateSteps_NextStepId",
                table: "AggregateSteps",
                column: "NextStepId",
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
    }
}
