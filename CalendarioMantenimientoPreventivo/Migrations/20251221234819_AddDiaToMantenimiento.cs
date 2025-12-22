using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarioMantenimientoPreventivo.Migrations
{
    /// <inheritdoc />
    public partial class AddDiaToMantenimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Dia",
                table: "Mantenimientos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dia",
                table: "Mantenimientos");
        }
    }
}
