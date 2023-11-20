using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FourCreate.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DataModelsUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Employment",
                schema: "FourCreate",
                table: "Employment");

            migrationBuilder.DropIndex(
                name: "IX_Employment_CompanyId",
                schema: "FourCreate",
                table: "Employment");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "FourCreate",
                table: "Employment");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employment",
                schema: "FourCreate",
                table: "Employment",
                columns: new[] { "CompanyId", "EmployeeId" });

            migrationBuilder.AddUniqueConstraint(
                name: "UQ_Employee_Email",
                schema: "FourCreate",
                table: "Employee",
                column: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Employment",
                schema: "FourCreate",
                table: "Employment");

            migrationBuilder.DropUniqueConstraint(
                name: "UQ_Employee_Email",
                schema: "FourCreate",
                table: "Employee");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "FourCreate",
                table: "Employment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employment",
                schema: "FourCreate",
                table: "Employment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Employment_CompanyId",
                schema: "FourCreate",
                table: "Employment",
                column: "CompanyId");
        }
    }
}
