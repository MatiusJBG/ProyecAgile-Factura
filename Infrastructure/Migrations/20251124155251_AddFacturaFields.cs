using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFacturaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CVV",
                table: "facturas",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Establecimiento_Fac",
                table: "facturas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "facturas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FechaVencimiento",
                table: "facturas",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Forma_Pago",
                table: "facturas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NumeroTarjeta",
                table: "facturas",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Plazo_Pago_Dias",
                table: "facturas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Punto_Venta",
                table: "facturas",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TitularTarjeta",
                table: "facturas",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "Valor_Pago",
                table: "facturas",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CVV",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "Establecimiento_Fac",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "FechaVencimiento",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "Forma_Pago",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "NumeroTarjeta",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "Plazo_Pago_Dias",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "Punto_Venta",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "TitularTarjeta",
                table: "facturas");

            migrationBuilder.DropColumn(
                name: "Valor_Pago",
                table: "facturas");
        }
    }
}
