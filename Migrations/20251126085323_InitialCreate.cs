using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceAggregator.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aggregates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aggregates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "POST"),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "REST"),
                    Status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AggregateSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NextStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregateSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregateSteps_AggregateSteps_NextStepId",
                        column: x => x.NextStepId,
                        principalTable: "AggregateSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AggregateSteps_AggregateSteps_SourceStepId",
                        column: x => x.SourceStepId,
                        principalTable: "AggregateSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AggregateSteps_Aggregates_AggregateId",
                        column: x => x.AggregateId,
                        principalTable: "Aggregates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AggregateSteps_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AggregateStepMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InputStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "AggregateInput"),
                    TargetField = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SourceField = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregateStepMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregateStepMappings_AggregateSteps_AggregateStepId",
                        column: x => x.AggregateStepId,
                        principalTable: "AggregateSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AggregateStepMappings_AggregateSteps_InputStepId",
                        column: x => x.InputStepId,
                        principalTable: "AggregateSteps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AggregateStepMappings_AggregateStepId",
                table: "AggregateStepMappings",
                column: "AggregateStepId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateStepMappings_InputStepId",
                table: "AggregateStepMappings",
                column: "InputStepId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateSteps_AggregateId",
                table: "AggregateSteps",
                column: "AggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateSteps_NextStepId",
                table: "AggregateSteps",
                column: "NextStepId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateSteps_ServiceId",
                table: "AggregateSteps",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateSteps_SourceStepId",
                table: "AggregateSteps",
                column: "SourceStepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregateStepMappings");

            migrationBuilder.DropTable(
                name: "AggregateSteps");

            migrationBuilder.DropTable(
                name: "Aggregates");

            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
