using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asteria.Data.Migrations
{
    public partial class added_post_collections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostCollections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    PostDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCollections", x => new { x.Id, x.PostId, x.CollectionId });
                    table.ForeignKey(
                        name: "FK_PostCollections_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostCollections_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostCollections_CollectionId",
                table: "PostCollections",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PostCollections_PostId",
                table: "PostCollections",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostCollections");
        }
    }
}
