using Microsoft.EntityFrameworkCore;
using TodoApi.Domain.Entities;

namespace TodoApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tarefa>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Id)
                .HasConversion<string>()
                .HasColumnType("text");

            e.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
            e.Property(t => t.Descricao).HasMaxLength(1000);
        });

        base.OnModelCreating(modelBuilder);
    }
}