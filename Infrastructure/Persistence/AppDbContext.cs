// Infrastructure/Persistence/AppDbContext.cs
using System;
using Core.Entities; 
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSets para las entidades (según ETAPA 5)
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetalleFactura> DetallesFactura { get; set; }
        public DbSet<ActualizacionPrecio> ActualizacionesPrecio { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------- PRODUCTO ----------
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("productos");

                entity.HasKey(e => e.Id_Pro);
                entity.Property(e => e.Id_Pro).HasColumnName("Id_Pro");
                
                entity.Property(e => e.Nom_Pro)
                      .HasColumnName("Nom_Pro")
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.Marca)
                      .HasColumnName("Marca")
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.Tip_Pro)
                      .HasColumnName("Tip_Pro")
                      .HasMaxLength(100);

                entity.Property(e => e.PrecioVenta)
                      .HasColumnName("PrecioVenta")
                      .HasColumnType("decimal(18,2)");

                // one-to-many: Producto -> Lotes
                entity.HasMany(p => p.Lotes)
                      .WithOne(l => l.Producto)
                      .HasForeignKey(l => l.Id_Pro)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------- LOTE ----------
            modelBuilder.Entity<Lote>(entity =>
            {
                entity.ToTable("lotes");

                entity.HasKey(e => e.Id_Lote);
                entity.Property(e => e.Id_Lote).HasColumnName("Id_Lote");

                entity.Property(e => e.Id_Pro).HasColumnName("Id_Pro").IsRequired();

                entity.Property(e => e.FechaEntrada)
                      .HasColumnName("FechaEntrada")
                      .IsRequired();

                entity.Property(e => e.CantidadEntrada)
                      .HasColumnName("CantidadEntrada")
                      .IsRequired();

                entity.Property(e => e.CantidadDisponible)
                      .HasColumnName("CantidadDisponible")
                      .IsRequired();

                entity.Property(e => e.PrecioUnidadCompra)
                      .HasColumnName("PrecioUnidadCompra")
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.PrecioTotalLote)
                      .HasColumnName("PrecioTotalLote")
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.FechaExpiracion)
                      .HasColumnName("FechaExpiracion")
                      .IsRequired(false);

                // Índice para optimizar selección FIFO (por producto y fecha de entrada)
                entity.HasIndex(e => new { e.Id_Pro, e.FechaEntrada })
                      .HasDatabaseName("IX_Lotes_IdPro_FechaEntrada");
            });

            // ---------- CLIENTE ----------
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("clientes");

                entity.HasKey(e => e.Id_Cli);
                entity.Property(e => e.Id_Cli).HasColumnName("Id_Cli");

                entity.Property(e => e.TipoCliente)
                      .HasColumnName("Tipo_Cliente")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.TipoDocumento)
                      .HasColumnName("Tipo_Documento")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.Documento)
                      .HasColumnName("Documento")
                      .HasMaxLength(30)
                      .IsRequired();

                entity.HasIndex(e => e.Documento)
                      .IsUnique()
                      .HasDatabaseName("UQ_Clientes_Documento");

                entity.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(255);
                entity.Property(e => e.Apellido).HasColumnName("Apellido").HasMaxLength(255);
                entity.Property(e => e.NombreEmpresa).HasColumnName("NombreEmpresa").HasMaxLength(255);
                entity.Property(e => e.Direccion).HasColumnName("Direccion").HasMaxLength(500);
                entity.Property(e => e.Correo).HasColumnName("Correo").HasMaxLength(255);
                entity.Property(e => e.Telefono).HasColumnName("Telefono").HasMaxLength(50);
            });

            // ---------- FACTURA ----------
            modelBuilder.Entity<Factura>(entity =>
            {
                entity.ToTable("facturas");

                entity.HasKey(e => e.Id_Fac);
                entity.Property(e => e.Id_Fac).HasColumnName("Id_Fac");

                entity.Property(e => e.Fecha)
                      .HasColumnName("Fec_Fac")
                      .IsRequired();

                entity.Property(e => e.Id_Cli)
                      .HasColumnName("Id_Cli")
                      .IsRequired(false);

                entity.Property(e => e.TotalSinIVA)
                      .HasColumnName("Tot_Fac_Sin_IVA")
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.IVA)
                      .HasColumnName("IVA_Fac")
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.TotalConIVA)
                      .HasColumnName("Tot_Fac_Con_IVA")
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Cliente)
                      .WithMany(c => c.Facturas)
                      .HasForeignKey(e => e.Id_Cli)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ---------- DETALLE FACTURA ----------
            modelBuilder.Entity<DetalleFactura>(entity =>
            {
                entity.ToTable("detallesfactura");

                entity.HasKey(e => e.Id_Det_Fac);
                entity.Property(e => e.Id_Det_Fac).HasColumnName("Id_Det_Fac");

                entity.Property(e => e.Id_Fac).HasColumnName("Id_Fac_Per").IsRequired();
                entity.Property(e => e.Id_Pro).HasColumnName("Id_Pro_Per").IsRequired();
                entity.Property(e => e.Id_Lote).HasColumnName("Id_Lote_Per").IsRequired(false);

                entity.Property(e => e.Cantidad).HasColumnName("Can_Com").IsRequired();

                entity.Property(e => e.PrecioUnitario)
                      .HasColumnName("Precio_Venta_Unit")
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Subtotal)
                      .HasColumnName("Precio_Venta_Total")
                      .HasColumnType("decimal(18,2)");

                // Relaciones
                entity.HasOne(d => d.Factura)
                      .WithMany(f => f.Detalles)
                      .HasForeignKey(d => d.Id_Fac)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Producto)
                      .WithMany()
                      .HasForeignKey(d => d.Id_Pro)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Lote)
                      .WithMany()
                      .HasForeignKey(d => d.Id_Lote)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ---------- ACTUALIZACION PRECIO (HISTORIAL) ----------
            modelBuilder.Entity<ActualizacionPrecio>(entity =>
            {
                entity.ToTable("precio_historial"); // o "actualizaciones" si prefieres ese nombre

                entity.HasKey(e => e.Id_Act_Pro);
                entity.Property(e => e.Id_Act_Pro).HasColumnName("Id_Act_Pro");

                entity.Property(e => e.Id_Pro).HasColumnName("Id_Pro").IsRequired(false);

                entity.Property(e => e.PrecioNuevo)
                      .HasColumnName("PrecioNuevo")
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.FechaActualizacion)
                      .HasColumnName("FechaActualizacion")
                      .IsRequired();

                entity.Property(e => e.Motivo)
                      .HasColumnName("Motivo")
                      .HasMaxLength(500);

                entity.HasOne(a => a.Producto)
                      .WithMany()
                      .HasForeignKey(a => a.Id_Pro)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ---------- AUDITORIA ----------
            modelBuilder.Entity<Auditoria>(entity =>
            {
                entity.ToTable("auditoria");

                entity.HasKey(e => e.Id_Aud);
                entity.Property(e => e.Id_Aud).HasColumnName("Id_Aud");

                entity.Property(e => e.Usuario).HasColumnName("Usuario").HasMaxLength(100);
                entity.Property(e => e.Accion).HasColumnName("Accion").HasMaxLength(100);
                entity.Property(e => e.Entidad).HasColumnName("Entidad").HasMaxLength(100);
                entity.Property(e => e.IdEntidad).HasColumnName("IdEntidad");
                entity.Property(e => e.Detalle).HasColumnName("Detalle").HasMaxLength(2000);
                entity.Property(e => e.Fecha).HasColumnName("Fecha");
            });

            // ---------- USUARIO (único) ----------
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");

                entity.HasKey(e => e.Id_Usu);
                entity.Property(e => e.Id_Usu).HasColumnName("Id_Usu");

                entity.Property(e => e.Nom_Usu)
                      .HasColumnName("Nom_Usu")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Contrasena_Usu)
                      .HasColumnName("Contrasena_Usu")
                      .HasMaxLength(255)
                      .IsRequired();

                // Nota: la limitación "solo un usuario" se aplica vía lógica de aplicación o trigger
            });

            // Registrar configuraciones adicionales si existen
            base.OnModelCreating(modelBuilder);
        }
    }
}
