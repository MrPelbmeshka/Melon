using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Melon.Migrations
{
    /// <inheritdoc />
    public partial class pick : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupPoint",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "PickupPointID",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PickupPoints",
                columns: table => new
                {
                    PickupPointID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupPoints", x => x.PickupPointID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PickupPointID",
                table: "Orders",
                column: "PickupPointID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PickupPoints_PickupPointID",
                table: "Orders",
                column: "PickupPointID",
                principalTable: "PickupPoints",
                principalColumn: "PickupPointID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PickupPoints_PickupPointID",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "PickupPoints");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PickupPointID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PickupPointID",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "PickupPoint",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
