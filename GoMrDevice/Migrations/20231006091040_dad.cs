using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoMrDevice.Migrations
{
    /// <inheritdoc />
    public partial class dad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RemovedDevices",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "RemovedDevices");
        }
    }
}
