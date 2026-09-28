using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceAggregator.Api.Migrations
{
    /// <inheritdoc />
    public partial class index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_ServiceMappings_ServiceId",
                table: "ServiceMappings",
                newName: "IX_ServiceMapping_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_AggregateSteps_AggregateId",
                table: "AggregateSteps",
                newName: "IX_AggregateStep_AggregateId");

            migrationBuilder.RenameIndex(
                name: "IX_AggregateStepMappings_AggregateStepId",
                table: "AggregateStepMappings",
                newName: "IX_AggregateStepMapping_AggregateStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionLog_TrackerId",
                table: "ExecutionLogs",
                column: "TrackerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExecutionLog_TrackerId",
                table: "ExecutionLogs");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceMapping_ServiceId",
                table: "ServiceMappings",
                newName: "IX_ServiceMappings_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_AggregateStep_AggregateId",
                table: "AggregateSteps",
                newName: "IX_AggregateSteps_AggregateId");

            migrationBuilder.RenameIndex(
                name: "IX_AggregateStepMapping_AggregateStepId",
                table: "AggregateStepMappings",
                newName: "IX_AggregateStepMappings_AggregateStepId");
        }
    }
}
