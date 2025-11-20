using Investimentos.Domain.Entities;
using Investimentos.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infrastructure.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente>? Clientes { get; set; }
    public DbSet<Produto>? Produtos { get; set; }
    public DbSet<Simulacao>? Simulacoes { get; set; }
    public DbSet<Investimento>? Investimentos { get; set; }
    public DbSet<RegistroTelemetria> RegistrosTelemetria { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relacionamento Investimento → Produto
        modelBuilder.Entity<Investimento>()
            .HasOne(i => i.Produto)
            .WithMany()
            .HasForeignKey(i => i.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}