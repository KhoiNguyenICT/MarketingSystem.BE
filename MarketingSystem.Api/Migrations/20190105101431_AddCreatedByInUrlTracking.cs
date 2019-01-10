using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingSystem.Api.Migrations
{
    public partial class AddCreatedByInUrlTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "UrlTrackings",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UrlTrackings_CreatedById",
                table: "UrlTrackings",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_UrlTrackings_AspNetUsers_CreatedById",
                table: "UrlTrackings",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UrlTrackings_AspNetUsers_CreatedById",
                table: "UrlTrackings");

            migrationBuilder.DropIndex(
                name: "IX_UrlTrackings_CreatedById",
                table: "UrlTrackings");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "UrlTrackings");
        }
    }
}
