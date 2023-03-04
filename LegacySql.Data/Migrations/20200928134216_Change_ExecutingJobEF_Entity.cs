using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LegacySql.Data.Migrations
{
    public partial class Change_ExecutingJobEF_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExecutingJobs",
                schema: "public",
                table: "ExecutingJobs");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ExecutingJobs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExecutingJobs",
                schema: "public",
                table: "ExecutingJobs",
                column: "JobType");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutingJobs_JobType",
                schema: "public",
                table: "ExecutingJobs",
                column: "JobType",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExecutingJobs",
                schema: "public",
                table: "ExecutingJobs");

            migrationBuilder.DropIndex(
                name: "IX_ExecutingJobs_JobType",
                schema: "public",
                table: "ExecutingJobs");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ExecutingJobs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExecutingJobs",
                schema: "public",
                table: "ExecutingJobs",
                column: "Id");
        }
    }
}
