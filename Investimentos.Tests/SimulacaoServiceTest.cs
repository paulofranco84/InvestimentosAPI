using Investimentos.Application.DTOs;
using Investimentos.Application.Services;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Moq;
using Xunit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Investimentos.Application.Tests
{
    public class SimulacaoServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUof;
        private readonly Mock<IProdutoRepository> _mockProdutoRepo;
        private readonly Mock<ISimulacaoRepository> _mockSimulacaoRepo;
        private readonly SimulacaoService _simulacaoService;

        public SimulacaoServiceTests()
        {
            _mockUof = new Mock<IUnitOfWork>();
            _mockProdutoRepo = new Mock<IProdutoRepository>();
            _mockSimulacaoRepo = new Mock<ISimulacaoRepository>();

            _mockUof.Setup(u => u.ProdutoRepository).Returns(_mockProdutoRepo.Object);
            _mockUof.Setup(u => u.SimulacaoRepository).Returns(_mockSimulacaoRepo.Object);

            _simulacaoService = new SimulacaoService(_mockUof.Object);
        }

        private Produto CriarProdutoMock()
        {
            return new Produto
            {
                Id = 1,
                Nome = "CDB 100% CDI",
                Tipo = "Renda Fixa",
                Rentabilidade = 0.10m,
                Risco = "Baixo",
                PrazoMinimo = 12,
                PrazoMaximo = 60
            };
        }

        [Fact]
        public async Task Simular_DeveRetornarRespostaCorreta_QuandoSimulacaoValida()
        {
            // Arrange
            var produto = CriarProdutoMock();
            var request = new SimulacaoRequestDTO
            {
                ClienteId = 1,
                TipoProduto = "Renda Fixa",
                Valor = 1000,
                PrazoMeses = 12
            };

            _mockProdutoRepo.Setup(r => r.ObterPorTipoAsync(request.TipoProduto))
                .ReturnsAsync(produto);

            // Act
            var resultado = await _simulacaoService.SimularAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(produto.Nome, resultado.ProdutoValidado.Nome);
            Assert.Equal(request.PrazoMeses, resultado.ResultadoSimulacao.PrazoMeses);
        }

        [Fact]
        public async Task Simular_DeveLancarExcecao_QuandoProdutoNaoEncontrado()
        {
            // Arrange
            var request = new SimulacaoRequestDTO
            {
                ClienteId = 1,
                TipoProduto = "Inexistente",
                Valor = 1000,
                PrazoMeses = 12
            };

            _mockProdutoRepo.Setup(r => r.ObterPorTipoAsync(request.TipoProduto))
                .ReturnsAsync((Produto)null!);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _simulacaoService.SimularAsync(request));
            _mockUof.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Simular_DeveLancarExcecao_QuandoPrazoInvalido()
        {
            // Arrange
            var produto = CriarProdutoMock();
            var request = new SimulacaoRequestDTO
            {
                ClienteId = 1,
                TipoProduto = "Renda Fixa",
                Valor = 1000,
                PrazoMeses = 6 // Prazo mínimo é 12
            };

            _mockProdutoRepo.Setup(r => r.ObterPorTipoAsync(request.TipoProduto))
                .ReturnsAsync(produto);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<ArgumentException>(() => _simulacaoService.SimularAsync(request));
            Assert.Equal("Prazo Inválido.", excecao.Message);
            _mockUof.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Simular_DeveCalcularCorretamente_ParaUmAno()
        {
            // Arrange
            var produto = CriarProdutoMock(); // Rentabilidade = 10%
            var request = new SimulacaoRequestDTO
            {
                ClienteId = 1,
                TipoProduto = "Renda Fixa",
                Valor = 1000,
                PrazoMeses = 12
            };

            _mockProdutoRepo.Setup(r => r.ObterPorTipoAsync(request.TipoProduto))
                .ReturnsAsync(produto);

            var valorEsperado = 1100.00; // 1000 * (1 + 0.10)^1

            // Act
            var resultado = await _simulacaoService.SimularAsync(request);

            // Assert
            Assert.Equal(valorEsperado.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), resultado.ResultadoSimulacao.ValorFinal);
        }

        [Fact]
        public async Task Simular_DeveCalcularCorretamente_ParaDoisAnos()
        {
            // Arrange
            var produto = CriarProdutoMock(); // Rentabilidade = 10%
            var request = new SimulacaoRequestDTO
            {
                ClienteId = 1,
                TipoProduto = "Renda Fixa",
                Valor = 1000,
                PrazoMeses = 24
            };

            _mockProdutoRepo.Setup(r => r.ObterPorTipoAsync(request.TipoProduto))
                .ReturnsAsync(produto);

            var valorEsperado = 1210.00; // 1000 * (1 + 0.10)^2

            // Act
            var resultado = await _simulacaoService.SimularAsync(request);

            // Assert
            Assert.Equal(valorEsperado.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), resultado.ResultadoSimulacao.ValorFinal);
        }
    }
}