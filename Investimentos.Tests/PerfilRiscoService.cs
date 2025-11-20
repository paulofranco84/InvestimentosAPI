using Investimentos.Application.Services;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Investimentos.Application.Tests
{
    public class PerfilRiscoServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUof;
        private readonly Mock<IInvestimentoRepository> _mockInvestimentoRepo;
        private readonly PerfilRiscoService _perfilRiscoService;

        public PerfilRiscoServiceTests()
        {
            _mockUof = new Mock<IUnitOfWork>();
            _mockInvestimentoRepo = new Mock<IInvestimentoRepository>();

            _mockUof.Setup(u => u.InvestimentoRepository).Returns(_mockInvestimentoRepo.Object);

            _perfilRiscoService = new PerfilRiscoService(_mockUof.Object);
        }

        private List<Investimento> CriarInvestimentos(double total, int quantidade, int liquidezCount)
        {
            var investimentos = new List<Investimento>();
            var produtoAltoRisco = new Produto
            {
                Id = 999,
                Nome = "Fundo Cripto",
                Tipo = "Fundos",
                Rentabilidade = 0.20M,
                Risco = "Alto",
                PrazoMinimo = 1,
                PrazoMaximo = 36
            };

            var produtoBaixoRisco = new Produto
            {
                Id = 998,
                Nome = "CDB Caixa",
                Tipo = "CDB",
                Rentabilidade = 0.10M,
                Risco = "Baixo",
                PrazoMinimo = 1,
                PrazoMaximo = 36
            };

            for (int i = 0; i < quantidade; i++)
            {
                investimentos.Add(new Investimento
                {
                    Valor = total / quantidade,
                    Tipo = i < liquidezCount ? "CDB" : "Fundos",
                    Produto = i < liquidezCount ? produtoBaixoRisco : produtoAltoRisco
                });
            }

            return investimentos;
        }

        [Fact]
        public async Task CalcularPerfil_DeveRetornarConservador_QuandoPontuacaoBaixa()
        {
            // Arrange
            int clienteId = 1;
            var investimentos = CriarInvestimentos(10000.0, 1, 1); // 1 CDB (baixo risco), 1 Fundo (alto risco)
            _mockInvestimentoRepo.Setup(r => r.ObterPorClienteAsync(clienteId))
                .ReturnsAsync(investimentos);

            // Act
            var resultado = await _perfilRiscoService.CalcularPerfilAsync(clienteId);

            // Assert
            Assert.Equal("Conservador", resultado.Perfil);
            Assert.Equal(13, resultado.Pontuacao); // Recalculado
            Assert.Equal("Perfil focado em segurança e liquidez.", resultado.Descricao);
        }

        [Fact]
        public async Task CalcularPerfil_DeveRetornarModerado_QuandoPontuacaoMedia()
        {
            // Arrange
            int clienteId = 2;
            var investimentos = CriarInvestimentos(30000.0, 5, 3); // 3 CDB (baixo risco), 2 Fundos (alto risco)
            _mockInvestimentoRepo.Setup(r => r.ObterPorClienteAsync(clienteId))
                .ReturnsAsync(investimentos);

            // Act
            var resultado = await _perfilRiscoService.CalcularPerfilAsync(clienteId);

            // Assert
            Assert.Equal("Moderado", resultado.Perfil);
            Assert.Equal(49, resultado.Pontuacao); // Confirmado
            Assert.Equal("Perfil equilibrado entre segurança e rentabilidade.", resultado.Descricao);
        }

        [Fact]
        public async Task CalcularPerfil_DeveRetornarAgressivo_QuandoPontuacaoAlta()
        {
            // Arrange
            int clienteId = 3;
            var investimentos = CriarInvestimentos(50000.0, 10, 2); // 2 CDB (baixo risco), 8 Fundos (alto risco)
            _mockInvestimentoRepo.Setup(r => r.ObterPorClienteAsync(clienteId))
                .ReturnsAsync(investimentos);

            // Act
            var resultado = await _perfilRiscoService.CalcularPerfilAsync(clienteId);

            // Assert
            Assert.Equal("Agressivo", resultado.Perfil);
            Assert.Equal(88, resultado.Pontuacao); // Corrigido de 106 para 88
            Assert.Equal("Perfil voltado para alta rentabilidade e maior risco.", resultado.Descricao);
        }

        [Fact]
        public async Task CalcularPerfil_DeveRetornarConservador_QuandoNaoExistemInvestimentos()
        {
            // Arrange
            int clienteId = 4;
            var investimentos = new List<Investimento>();
            _mockInvestimentoRepo.Setup(r => r.ObterPorClienteAsync(clienteId))
                .ReturnsAsync(investimentos);

            // Act
            var resultado = await _perfilRiscoService.CalcularPerfilAsync(clienteId);

            // Assert
            Assert.Equal("Conservador", resultado.Perfil);
            Assert.Equal(0, resultado.Pontuacao);
        }
    }
}