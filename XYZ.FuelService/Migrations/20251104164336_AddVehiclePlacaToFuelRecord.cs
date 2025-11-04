using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XYZ.FuelService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehiclePlacaToFuelRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new VehiclePlaca column as nullable to allow safe backfill from existing VehicleId
            migrationBuilder.AddColumn<string>(
                name: "VehiclePlaca",
                table: "FuelRecords",
                type: "text",
                nullable: true);

            // Make TipoMaquinaria non-nullable if needed
            migrationBuilder.AlterColumn<string>(
                name: "TipoMaquinaria",
                table: "FuelRecords",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehiclePlaca",
                table: "FuelRecords");
            // Revert TipoMaquinaria to nullable
            migrationBuilder.AlterColumn<string>(
                name: "TipoMaquinaria",
                table: "FuelRecords",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
