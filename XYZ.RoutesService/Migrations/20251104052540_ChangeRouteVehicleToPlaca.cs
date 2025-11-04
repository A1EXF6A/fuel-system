using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XYZ.RoutesService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRouteVehicleToPlaca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Routes");

            migrationBuilder.AddColumn<string>(
                name: "VehiclePlaca",
                table: "Routes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehiclePlaca",
                table: "Routes");

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "Routes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
