using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ============================
        //        DBSETS
        // ============================
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<DetalleFactura> DetallesFactura { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Precio> Precios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======================================================
            // CLIENTE → FACTURAS  (1 - N)
            // ======================================================
            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Cliente)
                .WithMany(c => c.Facturas)
                .HasForeignKey(f => f.Id_Cli_Per);

            // ======================================================
            // FACTURA → DETALLES  (1 - N)
            // ======================================================
            modelBuilder.Entity<DetalleFactura>()
                .HasOne(df => df.Factura)
                .WithMany(f => f.DetallesFactura)
                .HasForeignKey(df => df.Id_Fac_Per);

            // ======================================================
            // PRODUCTO → LOTES  (1 - N)
            // ======================================================
            modelBuilder.Entity<Lote>()
                .HasOne(l => l.Producto)
                .WithMany(p => p.Lotes)
                .HasForeignKey(l => l.Id_Pro_Per);

            // ======================================================
            // PRODUCTO → PRECIOS  (1 - N)
            // ======================================================
            modelBuilder.Entity<Precio>()
                .HasOne(p => p.Producto)
                .WithMany(pr => pr.Precios)
                .HasForeignKey(p => p.Id_Pro_Per);

            // ======================================================
            // PRODUCTO → DETALLESFACTURA  (1 - N)
            // ======================================================
            modelBuilder.Entity<DetalleFactura>()
                .HasOne(df => df.Producto)
                .WithMany(p => p.Detalles)
                .HasForeignKey(df => df.Id_Pro_Per);

            // ======================================================
            // LOTE → DETALLESFACTURA  (1 - N)
            // ======================================================
            modelBuilder.Entity<DetalleFactura>()
                .HasOne(df => df.Lote)
                .WithMany(l => l.Detalles)
                .HasForeignKey(df => df.Id_Lote_Per);

            // ======================================================
            // AUDITORIA opcional → PRODUCTO
            // ======================================================
            modelBuilder.Entity<Auditoria>()
                .HasOne(a => a.Producto)
                .WithMany()
                .HasForeignKey(a => a.Id_Pro_Per)
                .OnDelete(DeleteBehavior.NoAction);

            // ======================================================
            // AUDITORIA opcional → LOTE
            // ======================================================
            modelBuilder.Entity<Auditoria>()
                .HasOne(a => a.Lote)
                .WithMany()
                .HasForeignKey(a => a.Id_Lote_Per)
                .OnDelete(DeleteBehavior.NoAction);

            // ======================================================
            // USUARIO → AUDITORIAS (1 - N)
            // ======================================================
            modelBuilder.Entity<Auditoria>()
                .Property(a => a.Usuario)
                .HasMaxLength(100);
        }
    }
}
