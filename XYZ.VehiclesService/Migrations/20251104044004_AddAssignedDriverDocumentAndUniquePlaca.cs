using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XYZ.VehiclesService.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedDriverDocumentAndUniquePlaca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedDriverDocument",
                table: "Vehicles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Placa",
                table: "Vehicles",
                column: "Placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Placa",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "AssignedDriverDocument",
                table: "Vehicles");
        }
    }
}
