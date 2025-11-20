using Moq;
using Microsoft.AspNetCore.Mvc;
using Investimentos.API.Controllers;
using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Entities;
using Investimentos.Application.DTOs;
using System.Linq.Expressions;

namespace Investimentos.API.Tests;

public class InvestimentosControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUof;
    private readonly InvestimentosController _controller;

    public InvestimentosControllerTests()
    {
        _mockUof = new Mock<IUnitOfWork>();
        _controller = new InvestimentosController(_mockUof.Object);
    }

    #region ObterPorCliente Tests

    [Fact]
    public async Task Obter_ClienteNaoEncontrado_RetornaNotFound()
    {
        _mockUof.Setup(u => u.ClienteRepository.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync((Cliente)null);

        var result = await _controller.Obter(1);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Cliente não cadastrado", notFoundResult.Value);
    }

    [Fact]
    public async Task Obter_ClienteEncontradoSemInvestimentos_RetornaListaVazia()
    {
        var cliente = new Cliente { Id = 130, Nome = "Teste" };

        _mockUof.Setup(u => u.ClienteRepository.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync(cliente);

        _mockUof.Setup(u => u.InvestimentoRepository.ObterPorClienteAsync(1))
                .ReturnsAsync(new List<Investimento>());

        var result = await _controller.Obter(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var lista = Assert.IsAssignableFrom<IEnumerable<InvestimentoResponseDTO>>(okResult.Value);
        Assert.Empty(lista);
    }

    [Fact]
    public async Task Obter_ClienteEncontradoComInvestimentos_RetornaLista()
    {
        var cliente = new Cliente { Id = 130, Nome = "Teste" };
        var investimentos = new List<Investimento> { new Investimento { Id = 1, ClienteId = 130 } };

        _mockUof.Setup(u => u.ClienteRepository.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync(cliente);

        _mockUof.Setup(u => u.InvestimentoRepository.ObterPorClienteAsync(1))
                .ReturnsAsync(investimentos);

        var result = await _controller.Obter(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var lista = Assert.IsAssignableFrom<IEnumerable<InvestimentoResponseDTO>>(okResult.Value);
        Assert.Single(lista);
    }

    #endregion

    #region Investir Tests

    [Fact]
    public async Task Investir_ClienteNaoEncontrado_RetornaBadRequest()
    {
        var dto = new InvestimentoRequestDTO { ClienteId = 123, ProdutoId = 101, Valor = 1000, PrazoMeses = 12 };

        _mockUof.Setup(u => u.ClienteRepository.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync((Cliente)null);

        var result = await _controller.Investir(dto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Cliente não encontrado", badRequestResult.Value);
    }

    [Fact]
    public async Task Investir_ProdutoNaoEncontrado_RetornaBadRequest()
    {
        var dto = new InvestimentoRequestDTO { ClienteId = 1, ProdutoId = 2, Valor = 1000, PrazoMeses = 12 };
        var cliente = new Cliente { Id = 1 };

        _mockUof.Setup(u => u.ClienteRepository.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync(cliente);

        _mockUof.Setup(u => u.ProdutoRepository.GetAsync(It.IsAny<Expression<Func<Produto, bool>>>()))
                .ReturnsAsync((Produto)null);

        var result = await _controller.Investir(dto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Produto não encontrado", badRequestResult.Value);
    }

    [Fact]
    public async Task Investir_PrazoInvalido_RetornaBadRequest()
    {
        var dto = new InvestimentoRequestDTO { ClienteId = 1, ProdutoId = 2, Valor = 1000, PrazoMeses = 5 };
        var cliente = new Cliente { Id = 1 };
        var produto = new Produto { Id = 2, PrazoMinimo = 10, PrazoMaximo = 20 };

        _mockUof.Setup(u => u.ClienteRepository.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync(cliente);

        _mockUof.Setup(u => u.ProdutoRepository.GetAsync(It.IsAny<Expression<Func<Produto, bool>>>()))
                .ReturnsAsync(produto);

        var result = await _controller.Investir(dto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Prazo inválido", badRequestResult.Value);
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_InvestimentoNaoEncontrado_RetornaNotFound()
    {
        _mockUof.Setup(u => u.InvestimentoRepository.GetAsync(It.IsAny<int>()))
                .ReturnsAsync((Investimento)null);

        var result = await _controller.GetAsync(1);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Investimento não encontrado", notFoundResult.Value);
    }

    [Fact]
    public async Task GetAsync_InvestimentoEncontrado_RetornaOk()
    {
        var investimento = new Investimento { Id = 1, ClienteId = 1, Valor = 1000 };

        _mockUof.Setup(u => u.InvestimentoRepository.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(investimento);

        var result = await _controller.GetAsync(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var retorno = Assert.IsType<Investimento>(okResult.Value);
        Assert.Equal(1, retorno.Id);
    }

    #endregion
}