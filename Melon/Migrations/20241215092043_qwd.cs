using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Melon.Migrations
{
    /// <inheritdoc />
    public partial class qwd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupPoint",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "PickupPointID",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PickupPointID",
                table: "Users",
                column: "PickupPointID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_PickupPoints_PickupPointID",
                table: "Users",
                column: "PickupPointID",
                principalTable: "PickupPoints",
                principalColumn: "PickupPointID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_PickupPoints_PickupPointID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PickupPointID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PickupPointID",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "PickupPoint",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
