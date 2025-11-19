using Investimentos.Domain.Entities;
using Investimentos.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Investimentos.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var context = new AppDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

        if (context.Produtos.Any()) return;

        context.Clientes.AddRange(
            new Cliente { Id = 123, Nome = "Cliente Caixa 1" },
            new Cliente { Id = 124, Nome = "Cliente Caixa 2" },
            new Cliente { Id = 125, Nome = "Cliente Caixa 3" },
            new Cliente { Id = 126, Nome = "Cliente Caixa 4" },
            new Cliente { Id = 127, Nome = "Cliente Caixa 5" }
        );


        context.Produtos.AddRange(
            new Produto { Id = 101, Nome = "CDB Caixa 2026", Tipo = "CDB", Rentabilidade = 0.12M, Risco = "Baixo", PrazoMinimo = 1, PrazoMaximo = 36 },
            new Produto { Id = 102, Nome = "Fundo XPTO", Tipo = "Fundo", Rentabilidade = 0.18M, Risco = "Medio", PrazoMinimo = 1, PrazoMaximo = 60 },
            new Produto { Id = 103, Nome = "LCI Caixa 2026", Tipo = "LCI", Rentabilidade = 0.13M, Risco = "Baixo", PrazoMinimo = 3, PrazoMaximo = 36 },
            new Produto { Id = 104, Nome = "LCA Agro Caixa 2026", Tipo = "LCA", Rentabilidade = 0.14M, Risco = "Baixo", PrazoMinimo = 3, PrazoMaximo = 60 },
            new Produto { Id = 105, Nome = "Tesouro Selic 2026", Tipo = "Tesouro", Rentabilidade = 0.10M, Risco = "Baixo", PrazoMinimo = 6, PrazoMaximo = 60 },
            new Produto { Id = 106, Nome = "Fundo Cripto Alto Risco", Tipo = "Cripto", Rentabilidade = 0.20M, Risco = "Alto", PrazoMinimo = 1, PrazoMaximo = 36 }
        );

        context.Investimentos.AddRange(
            new Investimento { ClienteId = 123, Tipo = "CDB", Valor = 5000, PrazoMeses = 12, Rentabilidade = 0.12M, Data = new DateTime(2025, 1, 15) },
            new Investimento { ClienteId = 124, Tipo = "Fundo", Valor = 3000, PrazoMeses = 24, Rentabilidade = 0.18M, Data = new DateTime(2025, 3, 10) },
            new Investimento { ClienteId = 124, Tipo = "LCI", Valor = 10000, PrazoMeses = 18, Rentabilidade = 0.13M, Data = new DateTime(2025, 4, 8) },
            new Investimento { ClienteId = 125, Tipo = "Cripto", Valor = 10000, PrazoMeses = 12, Rentabilidade = 0.20M, Data = new DateTime(2025, 4, 18) },
            new Investimento { ClienteId = 126, Tipo = "Tesouro", Valor = 20000, PrazoMeses = 6, Rentabilidade = 0.10M, Data = new DateTime(2025, 5, 10) },
            new Investimento { ClienteId = 127, Tipo = "LCA", Valor = 15000, PrazoMeses = 12, Rentabilidade = 0.14M, Data = new DateTime(2025, 5, 16) }
        );

        await context.SaveChangesAsync();
    }
}
