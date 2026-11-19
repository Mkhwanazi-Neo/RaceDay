using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceDay.API.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "PARTICIPANT",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannerImageUrl",
                table: "EVENT",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "PARTICIPANT");

            migrationBuilder.DropColumn(
                name: "BannerImageUrl",
                table: "EVENT");
        }
    }
}
