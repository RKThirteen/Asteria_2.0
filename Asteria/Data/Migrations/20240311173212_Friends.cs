using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asteria.Data.Migrations
{
    public partial class Friends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserApplicationUser");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FriendshipId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateAccepted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => new { x.Id, x.UserId, x.FriendshipId });
                    table.ForeignKey(
                        name: "FK_Friends_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Friends_AspNetUsers_FriendshipId",
                        column: x => x.FriendshipId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friends_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_ApplicationUserId",
                table: "Friends",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_FriendshipId",
                table: "Friends",
                column: "FriendshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_UserId",
                table: "Friends",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "ApplicationUserApplicationUser",
                columns: table => new
                {
                    ContactListId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FriendsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserApplicationUser", x => new { x.ContactListId, x.FriendsId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserApplicationUser_AspNetUsers_ContactListId",
                        column: x => x.ContactListId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserApplicationUser_AspNetUsers_FriendsId",
                        column: x => x.FriendsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserApplicationUser_FriendsId",
                table: "ApplicationUserApplicationUser",
                column: "FriendsId");
        }
    }
}
