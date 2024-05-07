using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGallery.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailFieldRenameHashtag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtworkArtworkTag");

            migrationBuilder.DropTable(
                name: "ArtworkTag");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserProfile",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Hashtag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TotalUses = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hashtag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArtworkHashtag",
                columns: table => new
                {
                    ArtworksId = table.Column<Guid>(type: "uuid", nullable: false),
                    HashtagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtworkHashtag", x => new { x.ArtworksId, x.HashtagsId });
                    table.ForeignKey(
                        name: "FK_ArtworkHashtag_Artwork_ArtworksId",
                        column: x => x.ArtworksId,
                        principalTable: "Artwork",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtworkHashtag_Hashtag_HashtagsId",
                        column: x => x.HashtagsId,
                        principalTable: "Hashtag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtworkHashtag_HashtagsId",
                table: "ArtworkHashtag",
                column: "HashtagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtworkHashtag");

            migrationBuilder.DropTable(
                name: "Hashtag");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserProfile");

            migrationBuilder.CreateTable(
                name: "ArtworkTag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TotalUses = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtworkTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArtworkArtworkTag",
                columns: table => new
                {
                    ArtworksId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtworkArtworkTag", x => new { x.ArtworksId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_ArtworkArtworkTag_ArtworkTag_TagsId",
                        column: x => x.TagsId,
                        principalTable: "ArtworkTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtworkArtworkTag_Artwork_ArtworksId",
                        column: x => x.ArtworksId,
                        principalTable: "Artwork",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtworkArtworkTag_TagsId",
                table: "ArtworkArtworkTag",
                column: "TagsId");
        }
    }
}
