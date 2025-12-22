using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarioMantenimientoPreventivo.Migrations
{
    /// <inheritdoc />
    public partial class AgregarParametrosYNotificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ParametrosSistema",
                columns: new[] { "Clave", "Valor" },
                values: new object[] { "NotificacionesActivas", "true" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ParametrosSistema",
                keyColumn: "Clave",
                keyValue: "NotificacionesActivas");
        }
    }
}
