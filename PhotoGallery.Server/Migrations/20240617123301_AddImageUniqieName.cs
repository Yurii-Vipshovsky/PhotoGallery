using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoGallery.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUniqieName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUniqueName",
                table: "Images",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUniqueName",
                table: "Images");
        }
    }
}
