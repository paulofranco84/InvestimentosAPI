using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Investimentos.API.Controllers;
using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Entities;
using Investimentos.Application.DTOs;
using Investimentos.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Investimentos.API.Tests;

public class SimulacoesControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUof;
    private readonly Mock<SimulacaoService> _mockSimulacaoService;
    private readonly SimulacoesController _controller;

    public SimulacoesControllerTests()
    {
        _mockUof = new Mock<IUnitOfWork>();
        _mockSimulacaoService = new Mock<SimulacaoService>(null); // SimulacaoService pode depender de algo, aqui passamos null para simplificar
        _controller = new SimulacoesController(_mockSimulacaoService.Object, _mockUof.Object);
    }

    #region Simular Tests

    [Fact]
    public async Task Simular_ClienteNaoEncontrado_RetornaBadRequest()
    {
        var request = new SimulacaoRequestDTO { ClienteId = 1, TipoProduto = "CDB", PrazoMeses = 12, Valor = 1000 };

        _mockUof.Setup(u => u.ClienteRepository.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync((Cliente)null);

        var result = await _controller.Simular(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Cliente não encontrado", badRequestResult.Value);
    }

    [Fact]
    public async Task Simular_ProdutoNaoEncontrado_RetornaBadRequest()
    {
        var request = new SimulacaoRequestDTO { ClienteId = 1, TipoProduto = "CDB", PrazoMeses = 12, Valor = 1000 };
        var cliente = new Cliente { Id = 1 };

        _mockUof.Setup(u => u.ClienteRepository.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync(cliente);

        _mockUof.Setup(u => u.ProdutoRepository.GetAsync(It.IsAny<Expression<Func<Produto, bool>>>()))
                .ReturnsAsync((Produto)null);

        var result = await _controller.Simular(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Produto com tipo e prazo informados não encontrados", badRequestResult.Value);
    }

    [Fact]
    public async Task Simular_PrazoInvalido_RetornaBadRequest()
    {
        var request = new SimulacaoRequestDTO { ClienteId = 1, TipoProduto = "CDB", PrazoMeses = 5, Valor = 1000 };
        var cliente = new Cliente { Id = 1 };
        var produto = new Produto { Id = 2, Tipo = "CDB", PrazoMinimo = 10, PrazoMaximo = 20 , Nome = "CDB Caixa 2026", Rentabilidade = 0.20M, Risco = "Baixo"};

        _mockUof.Setup(u => u.ClienteRepository.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync(cliente);

        _mockUof.Setup(u => u.ProdutoRepository.GetAsync(It.IsAny<Expression<Func<Produto, bool>>>()))
                .ReturnsAsync(produto);

        var result = await _controller.Simular(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Prazo inválido", badRequestResult.Value);
    }

    #endregion

    #region ObterSimulacoes Tests

    [Fact]
    public async Task ObterSimulacoes_RetornaListaDeSimulacoes()
    {
        var simulacoes = new List<Simulacao>
        {
            new Simulacao
            {
                Id = 1,
                ClienteId = 1,
                Produto = new Produto { Nome = "CDB" },
                ValorInvestido = 1000,
                ValorFinal = 1200,
                PrazoMeses = 12,
                DataSimulacao = DateTime.UtcNow
            }
        };

        _mockUof.Setup(u => u.SimulacaoRepository.ObterTodasAsync())
                .ReturnsAsync(simulacoes);

        var result = await _controller.ObterSimulacoes();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var lista = Assert.IsAssignableFrom<IEnumerable<SimulacoesResponseDTO>>(okResult.Value);
        Assert.Single(lista);
        Assert.Equal("CDB", lista.First().Produto);
    }

    #endregion

    #region SimulacoesPorProdutoEDia Tests

    [Fact]
    public async Task SimulacoesPorProdutoEDia_RetornaDados()
    {
        var dados = new List<object>
        {
            new { Produto = "CDB", Dia = DateTime.UtcNow.Date, Quantidade = 5 }
        };

        _mockUof.Setup(u => u.SimulacaoRepository.ObterSimulacoesPorProdutoDiaAsync())
                .ReturnsAsync(dados);

        var result = await _controller.SimulacoesPorProdutoEDia();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dados, okResult.Value);
    }

    #endregion
}