using Microsoft.EntityFrameworkCore;
using TodoApi.Domain.Entities;

namespace TodoApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tarefa>(e =>
        {
            e.HasKey(t => t.Id);

            e.Property(t => t.Id)
                .HasConversion<string>()
                .HasColumnType("text");

            e.Property(t => t.UsuarioId)
                .HasConversion<string>()
                .HasColumnType("text");

            e.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
            e.Property(t => t.Descricao).HasMaxLength(1000);
            e.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasKey(u => u.Id);

            e.Property(u => u.Id)
                .HasConversion<string>()
                .HasColumnType("text");

            e.Property(u => u.Nome).IsRequired().HasMaxLength(100);
            e.Property(u => u.Email).IsRequired().HasMaxLength(200);
            e.Property(u => u.SenhaHash).IsRequired().HasMaxLength(500);

            e.HasIndex(u => u.Email).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}