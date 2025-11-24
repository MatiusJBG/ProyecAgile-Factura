using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    Id_Cli = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tipo_Cliente = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tipo_Documento = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Num_Documento = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Apellido = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direccion = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Correo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.Id_Cli);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    Id_Pro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tip_Pro = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nom_Pro = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Marca = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos", x => x.Id_Pro);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id_Usu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom_Usu = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contrasena_Usu = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id_Usu);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "facturas",
                columns: table => new
                {
                    Id_Fac = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Fec_Fac = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Id_Cli_Per = table.Column<int>(type: "int", nullable: false),
                    Tot_Fac_Sin_IVA = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    IVA_Fac = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Tot_Fac_Con_IVA = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facturas", x => x.Id_Fac);
                    table.ForeignKey(
                        name: "FK_facturas_clientes_Id_Cli_Per",
                        column: x => x.Id_Cli_Per,
                        principalTable: "clientes",
                        principalColumn: "Id_Cli",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "lotes",
                columns: table => new
                {
                    Id_Lote = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Id_Pro_Per = table.Column<int>(type: "int", nullable: false),
                    Fec_Ent = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Fec_Exp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Cantidad_Recibida = table.Column<int>(type: "int", nullable: false),
                    Cantidad_Disponible = table.Column<int>(type: "int", nullable: false),
                    Precio_Unitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Precio_Lote = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, computedColumnSql: "(`Cantidad_Recibida` * `Precio_Unitario`)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lotes", x => x.Id_Lote);
                    table.ForeignKey(
                        name: "FK_lotes_productos_Id_Pro_Per",
                        column: x => x.Id_Pro_Per,
                        principalTable: "productos",
                        principalColumn: "Id_Pro",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "precios",
                columns: table => new
                {
                    Id_Precio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Id_Pro_Per = table.Column<int>(type: "int", nullable: false),
                    Precio_Venta = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Fecha_Actualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Motivo = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_precios", x => x.Id_Precio);
                    table.ForeignKey(
                        name: "FK_precios_productos_Id_Pro_Per",
                        column: x => x.Id_Pro_Per,
                        principalTable: "productos",
                        principalColumn: "Id_Pro",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "auditoria",
                columns: table => new
                {
                    Id_Aud = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Tipo_Accion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Id_Pro_Per = table.Column<int>(type: "int", nullable: true),
                    Id_Lote_Per = table.Column<int>(type: "int", nullable: true),
                    Usuario = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria", x => x.Id_Aud);
                    table.ForeignKey(
                        name: "FK_auditoria_lotes_Id_Lote_Per",
                        column: x => x.Id_Lote_Per,
                        principalTable: "lotes",
                        principalColumn: "Id_Lote",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_auditoria_productos_Id_Pro_Per",
                        column: x => x.Id_Pro_Per,
                        principalTable: "productos",
                        principalColumn: "Id_Pro",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "detallesfactura",
                columns: table => new
                {
                    Id_Det_Fac = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Id_Fac_Per = table.Column<int>(type: "int", nullable: false),
                    Id_Lote_Per = table.Column<int>(type: "int", nullable: false),
                    Id_Pro_Per = table.Column<int>(type: "int", nullable: false),
                    Cantidad_Comprada = table.Column<int>(type: "int", nullable: false),
                    Precio_Venta_Unit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Precio_Venta_Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, computedColumnSql: "(`Cantidad_Comprada` * `Precio_Venta_Unit`)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detallesfactura", x => x.Id_Det_Fac);
                    table.ForeignKey(
                        name: "FK_detallesfactura_facturas_Id_Fac_Per",
                        column: x => x.Id_Fac_Per,
                        principalTable: "facturas",
                        principalColumn: "Id_Fac",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detallesfactura_lotes_Id_Lote_Per",
                        column: x => x.Id_Lote_Per,
                        principalTable: "lotes",
                        principalColumn: "Id_Lote",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_detallesfactura_productos_Id_Pro_Per",
                        column: x => x.Id_Pro_Per,
                        principalTable: "productos",
                        principalColumn: "Id_Pro",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_Id_Lote_Per",
                table: "auditoria",
                column: "Id_Lote_Per");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_Id_Pro_Per",
                table: "auditoria",
                column: "Id_Pro_Per");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_Num_Documento",
                table: "clientes",
                column: "Num_Documento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_detallesfactura_Id_Fac_Per",
                table: "detallesfactura",
                column: "Id_Fac_Per");

            migrationBuilder.CreateIndex(
                name: "IX_detallesfactura_Id_Lote_Per",
                table: "detallesfactura",
                column: "Id_Lote_Per");

            migrationBuilder.CreateIndex(
                name: "IX_detallesfactura_Id_Pro_Per",
                table: "detallesfactura",
                column: "Id_Pro_Per");

            migrationBuilder.CreateIndex(
                name: "IX_facturas_Id_Cli_Per",
                table: "facturas",
                column: "Id_Cli_Per");

            migrationBuilder.CreateIndex(
                name: "IX_lotes_Id_Pro_Per",
                table: "lotes",
                column: "Id_Pro_Per");

            migrationBuilder.CreateIndex(
                name: "IX_precios_Id_Pro_Per",
                table: "precios",
                column: "Id_Pro_Per");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditoria");

            migrationBuilder.DropTable(
                name: "detallesfactura");

            migrationBuilder.DropTable(
                name: "precios");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "facturas");

            migrationBuilder.DropTable(
                name: "lotes");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "productos");
        }
    }
}
