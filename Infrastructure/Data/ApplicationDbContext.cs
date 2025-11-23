using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetalleFactura> DetallesFactura { get; set; }
        public DbSet<Precio> Precios { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("clientes");
                entity.HasKey(e => e.Id_Cli);
                
                entity.Property(e => e.Tipo_Cliente)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                
                entity.Property(e => e.Tipo_Documento)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                
                entity.Property(e => e.Num_Documento)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.HasIndex(e => e.Num_Documento)
                    .IsUnique();
                
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.Apellido)
                    .HasMaxLength(255);
                
                entity.Property(e => e.Direccion)
                    .HasMaxLength(255);
                
                entity.Property(e => e.Correo)
                    .HasMaxLength(255);
                
                entity.Property(e => e.Telefono)
                    .HasMaxLength(20);
            });

            // Configuración de Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("productos");
                entity.HasKey(e => e.Id_Pro);
                
                entity.Property(e => e.Tip_Pro)
                    .IsRequired()
                    .HasMaxLength(30);
                
                entity.Property(e => e.Nom_Pro)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Marca)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            // Configuración de Lote
            modelBuilder.Entity<Lote>(entity =>
            {
                entity.ToTable("lotes");
                entity.HasKey(e => e.Id_Lote);
                
                entity.Property(e => e.Fec_Ent)
                    .IsRequired();
                
                entity.Property(e => e.Fec_Exp)
                    .IsRequired();
                
                entity.Property(e => e.Cantidad_Recibida)
                    .IsRequired();
                
                entity.Property(e => e.Cantidad_Disponible)
                    .IsRequired();
                
                entity.Property(e => e.Precio_Unitario)
                    .HasPrecision(18, 2)
                    .IsRequired();
                
                // Campo generado - no se inserta ni actualiza
                entity.Property(e => e.Precio_Lote)
                    .HasPrecision(18, 2)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasComputedColumnSql("(`Cantidad_Recibida` * `Precio_Unitario`)");
                
                // Relación con Producto
                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.Lotes)
                    .HasForeignKey(e => e.Id_Pro_Per)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Factura
            modelBuilder.Entity<Factura>(entity =>
            {
                entity.ToTable("facturas");
                entity.HasKey(e => e.Id_Fac);
                
                entity.Property(e => e.Fec_Fac)
                    .IsRequired();
                
                entity.Property(e => e.Tot_Fac_Sin_IVA)
                    .HasPrecision(10, 2);
                
                entity.Property(e => e.IVA_Fac)
                    .HasPrecision(10, 2);
                
                entity.Property(e => e.Tot_Fac_Con_IVA)
                    .HasPrecision(10, 2);
                
                // Relación con Cliente
                entity.HasOne(e => e.Cliente)
                    .WithMany(c => c.Facturas)
                    .HasForeignKey(e => e.Id_Cli_Per)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de DetalleFactura
            modelBuilder.Entity<DetalleFactura>(entity =>
            {
                entity.ToTable("detallesfactura");
                entity.HasKey(e => e.Id_Det_Fac);
                
                entity.Property(e => e.Cantidad_Comprada)
                    .IsRequired();
                
                entity.Property(e => e.Precio_Venta_Unit)
                    .HasPrecision(18, 2)
                    .IsRequired();
                
                // Campo generado - no se inserta ni actualiza
                entity.Property(e => e.Precio_Venta_Total)
                    .HasPrecision(18, 2)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasComputedColumnSql("(`Cantidad_Comprada` * `Precio_Venta_Unit`)");
                
                // Relación con Factura
                entity.HasOne(e => e.Factura)
                    .WithMany(f => f.Detalles)
                    .HasForeignKey(e => e.Id_Fac_Per)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Relación con Lote
                entity.HasOne(e => e.Lote)
                    .WithMany(l => l.DetallesFactura)
                    .HasForeignKey(e => e.Id_Lote_Per)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Relación con Producto
                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.DetallesFactura)
                    .HasForeignKey(e => e.Id_Pro_Per)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Precio
            modelBuilder.Entity<Precio>(entity =>
            {
                entity.ToTable("precios");
                entity.HasKey(e => e.Id_Precio);
                
                entity.Property(e => e.Precio_Venta)
                    .HasPrecision(18, 2)
                    .IsRequired();
                
                entity.Property(e => e.Fecha_Actualizacion)
                    .IsRequired();
                
                entity.Property(e => e.Motivo)
                    .HasMaxLength(60);
                
                // Relación con Producto
                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.Precios)
                    .HasForeignKey(e => e.Id_Pro_Per)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Auditoria
            modelBuilder.Entity<Auditoria>(entity =>
            {
                entity.ToTable("auditoria");
                entity.HasKey(e => e.Id_Aud);
                
                entity.Property(e => e.Fecha)
                    .IsRequired();
                
                entity.Property(e => e.Tipo_Accion)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnType("text");
                
                entity.Property(e => e.Usuario)
                    .HasMaxLength(100);
                
                // Relación con Producto (opcional)
                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.Auditorias)
                    .HasForeignKey(e => e.Id_Pro_Per)
                    .OnDelete(DeleteBehavior.SetNull);
                
                // Relación con Lote (opcional)
                entity.HasOne(e => e.Lote)
                    .WithMany(l => l.Auditorias)
                    .HasForeignKey(e => e.Id_Lote_Per)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");
                entity.HasKey(e => e.Id_Usu);
                
                entity.Property(e => e.Nom_Usu)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Contrasena_Usu)
                    .IsRequired()
                    .HasMaxLength(255);
            });
        }
    }
}
