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
            for (int i = 0; i < quantidade; i++)
            {
                investimentos.Add(new Investimento
                {
                    Valor = total / quantidade,
                    Tipo = i < liquidezCount ? "CDB" : "Fundos"
                });
            }
            return investimentos;
        }

        [Fact]
        public async Task CalcularPerfil_DeveRetornarConservador_QuandoPontuacaoBaixa()
        {
            // Arrange
            int clienteId = 1;
            var investimentos = CriarInvestimentos(10000.0, 2, 1); // Pontuação esperada: 23
            _mockInvestimentoRepo.Setup(r => r.ObterPorClienteAsync(clienteId))
                .ReturnsAsync(investimentos);

            // Act
            var resultado = await _perfilRiscoService.CalcularPerfilAsync(clienteId);

            // Assert
            Assert.Equal("Conservador", resultado.Perfil);
            Assert.Equal(23, resultado.Pontuacao);
            Assert.Equal("Perfil focado em segurança e liquidez.", resultado.Descricao);
        }

        [Fact]
        public async Task CalcularPerfil_DeveRetornarModerado_QuandoPontuacaoMedia()
        {
            // Arrange
            int clienteId = 2;
            var investimentos = CriarInvestimentos(30000.0, 5, 3); // Pontuação esperada: 64
            _mockInvestimentoRepo.Setup(r => r.ObterPorClienteAsync(clienteId))
                .ReturnsAsync(investimentos);

            // Act
            var resultado = await _perfilRiscoService.CalcularPerfilAsync(clienteId);

            // Assert
            Assert.Equal("Moderado", resultado.Perfil);
            Assert.Equal(64, resultado.Pontuacao);
            Assert.Equal("Perfil equilibrado entre segurança e rentabilidade.", resultado.Descricao);
        }

        [Fact]
        public async Task CalcularPerfil_DeveRetornarAgressivo_QuandoPontuacaoAlta()
        {
            // Arrange
            int clienteId = 3;
            var investimentos = CriarInvestimentos(50000.0, 10, 2); // Pontuação esperada: 106
            _mockInvestimentoRepo.Setup(r => r.ObterPorClienteAsync(clienteId))
                .ReturnsAsync(investimentos);

            // Act
            var resultado = await _perfilRiscoService.CalcularPerfilAsync(clienteId);

            // Assert
            Assert.Equal("Agressivo", resultado.Perfil);
            Assert.Equal(106, resultado.Pontuacao);
            Assert.Equal("Perfil voltado para alta rentabilidade e maior risco.", resultado.Descricao);
        }

        [Fact]
        public async Task CalcularPerfil_DeveRetornarConservador_QuandoNaoExistemInvestimentos()
        {
            // Arrange
            int clienteId = 4;
            var investimentos = new List<Investimento>(); // Pontuação esperada: 0
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