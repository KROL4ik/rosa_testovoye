using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rosa_testovoye.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Department = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Role = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificateRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    CopiesCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CustomTypeName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestStatusHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    FromStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    ToStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ChangedByEmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ChangedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestStatusHistory_CertificateRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "CertificateRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestStatusHistory_Employees_ChangedByEmployeeId",
                        column: x => x.ChangedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequests_EmployeeId_Type_Status",
                table: "CertificateRequests",
                columns: new[] { "EmployeeId", "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestStatusHistory_ChangedByEmployeeId",
                table: "RequestStatusHistory",
                column: "ChangedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestStatusHistory_RequestId",
                table: "RequestStatusHistory",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestStatusHistory");

            migrationBuilder.DropTable(
                name: "CertificateRequests");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
