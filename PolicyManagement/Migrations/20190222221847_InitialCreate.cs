using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PolicyManagement.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Policy",
                columns: table => new
                {
                    PolicyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Administrator = table.Column<string>(nullable: true),
                    AdvisoryCommittee = table.Column<string>(nullable: true),
                    AmendAuth = table.Column<string>(nullable: true),
                    AmendDate = table.Column<DateTime>(nullable: false),
                    ApprDate = table.Column<DateTime>(nullable: false),
                    ApprovalAuthority = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    NxtReviewDate = table.Column<DateTime>(nullable: false),
                    OrigApprAuth = table.Column<string>(nullable: true),
                    PDescription = table.Column<string>(nullable: true),
                    PScope = table.Column<string>(nullable: true),
                    PTitle = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policy", x => x.PolicyId);
                });

            migrationBuilder.CreateTable(
                name: "Procedure",
                columns: table => new
                {
                    ProcedureId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PolicyId = table.Column<int>(nullable: false),
                    PrDesc = table.Column<string>(nullable: true),
                    PrPurpose = table.Column<string>(nullable: true),
                    PrTitle = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure", x => x.ProcedureId);
                    table.ForeignKey(
                        name: "FK_Procedure_Policy_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policy",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Process",
                columns: table => new
                {
                    ProcessId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProcDesc = table.Column<string>(nullable: true),
                    ProcTitle = table.Column<string>(nullable: true),
                    ProcedureId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Process", x => x.ProcessId);
                    table.ForeignKey(
                        name: "FK_Process_Procedure_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedure",
                        principalColumn: "ProcedureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    ActionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ADesc = table.Column<string>(nullable: true),
                    ATitle = table.Column<string>(nullable: true),
                    Departments = table.Column<string>(nullable: true),
                    Inputs = table.Column<string>(nullable: true),
                    IsSRSAffected = table.Column<bool>(nullable: false),
                    Outputs = table.Column<string>(nullable: true),
                    ProcessId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.ActionId);
                    table.ForeignKey(
                        name: "FK_Actions_Process_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "Process",
                        principalColumn: "ProcessId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actions_ProcessId",
                table: "Actions",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_PolicyId",
                table: "Procedure",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Process_ProcedureId",
                table: "Process",
                column: "ProcedureId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "Process");

            migrationBuilder.DropTable(
                name: "Procedure");

            migrationBuilder.DropTable(
                name: "Policy");
        }
    }
}
