using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XYZ.DriversService.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedVehiclePlaca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignedVehicleId",
                table: "Drivers",
                newName: "AssignedVehiclePlaca");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignedVehiclePlaca",
                table: "Drivers",
                newName: "AssignedVehicleId");
        }
    }
}
