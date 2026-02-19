using Microsoft.EntityFrameworkCore;
using DesafioFinal.Models;

namespace DesafioFinal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Equipamento> Equipamentos => Set<Equipamento>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.Entity<Equipamento>(e =>
            {
                e.ToTable("equipamentos");

                e.HasKey(x => x.Id);

                e.Property(x => x.Codigo)
                    .HasMaxLength(50)
                    .IsRequired();

                e.HasIndex(x => x.Codigo)
                    .IsUnique();

                e.Property(x => x.Modelo)
                    .HasMaxLength(120)
                    .IsRequired();

                e.Property(x => x.LocalizacaoAtual)
                    .HasMaxLength(200)
                    .IsRequired();

                e.Property(x => x.Horimetro)
                    .HasColumnType("numeric(12,2)");

                e.Property(x => x.Tipo)
                    .HasConversion<int>();

                e.Property(x => x.StatusOperacional)
                    .HasConversion<int>();
            });
        }
    }
}
