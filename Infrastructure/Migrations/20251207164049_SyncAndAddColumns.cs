using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncAndAddColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Manual sync: Add columns to existing Facturas table
            migrationBuilder.Sql("ALTER TABLE `Facturas` ADD COLUMN `Estado` int NOT NULL DEFAULT 0;");
            migrationBuilder.Sql("ALTER TABLE `Facturas` ADD COLUMN `ClaveAcceso` longtext NULL;");
            migrationBuilder.Sql("ALTER TABLE `Facturas` ADD COLUMN `MensajeError` longtext NULL;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.Sql("ALTER TABLE `Facturas` DROP COLUMN `Estado`;");
             migrationBuilder.Sql("ALTER TABLE `Facturas` DROP COLUMN `ClaveAcceso`;");
             migrationBuilder.Sql("ALTER TABLE `Facturas` DROP COLUMN `MensajeError`;");
        }
    }
}
