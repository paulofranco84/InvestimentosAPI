using Investimentos.API.Controllers;
using Investimentos.Application.DTOs;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;

namespace Investimentos.API.Tests;
public class ProdutosControllerTests
{
    private readonly Mock<IUnitOfWork> _uofMock;
    private readonly Mock<IClienteRepository> _clienteRepoMock;
    private readonly Mock<IProdutoRepository> _produtoRepoMock;
    private readonly Mock<IPerfilRiscoService> _perfilServiceMock;
    private readonly ProdutosController _controller;

    public ProdutosControllerTests()
    {
        _uofMock = new Mock<IUnitOfWork>();
        _clienteRepoMock = new Mock<IClienteRepository>();
        _produtoRepoMock = new Mock<IProdutoRepository>();
        _perfilServiceMock = new Mock<IPerfilRiscoService>();

        _uofMock.Setup(u => u.ClienteRepository).Returns(_clienteRepoMock.Object);
        _uofMock.Setup(u => u.ProdutoRepository).Returns(_produtoRepoMock.Object);

        _controller = new ProdutosController(_uofMock.Object, _perfilServiceMock.Object);
    }

    [Fact]
    public async Task RecomendadosCliente_ClienteNaoEncontrado_RetornaBadRequest()
    {
        // Arrange
        _clienteRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(new Cliente { Id = 1 });


        // Act
        var resultado = await _controller.RecomendadosCliente(1);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Não foi possível calcular o perfil do cliente", badRequest.Value);
    }

    [Fact]
    public async Task RecomendadosCliente_PerfilNaoCalculado_RetornaBadRequest()
    {
        // Arrange
        _clienteRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(new Cliente { Id = 1 });

        _perfilServiceMock.Setup(s => s.CalcularPerfilAsync(1))
            .ReturnsAsync((PerfilRisco)null);

        // Act
        var resultado = await _controller.RecomendadosCliente(1);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Não foi possível calcular o perfil do cliente", badRequest.Value);
    }

    [Fact]
    public async Task RecomendadosCliente_SemProdutosParaPerfil_RetornaBadRequest()
    {
        // Arrange
        _clienteRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(new Cliente { Id = 1 });

        _perfilServiceMock.Setup(s => s.CalcularPerfilAsync(1))
            .ReturnsAsync(new PerfilRisco { Perfil = "conservador" });

        _produtoRepoMock.Setup(r => r.ObterTop3PorPerfilAsync("conservador"))
            .ReturnsAsync(new List<Produto>());

        // Act
        var resultado = await _controller.RecomendadosCliente(1);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Perfil inválido", badRequest.Value);
    }

    [Fact]
    public async Task RecomendadosCliente_DadosValidos_RetornaOkComProdutos()
    {
        // Arrange
        _clienteRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(new Cliente { Id = 1 });

        _perfilServiceMock.Setup(s => s.CalcularPerfilAsync(1))
            .ReturnsAsync(new PerfilRisco { Perfil = "moderado" });

        var produtos = new List<Produto>
        {
            new Produto { Id = 1, Nome = "Produto A", Tipo = "Fundo", Rentabilidade = 10, Risco = "Baixo" },
            new Produto { Id = 2, Nome = "Produto B", Tipo = "Fundo", Rentabilidade = 12, Risco = "Médio" }
        };

        _produtoRepoMock.Setup(r => r.ObterTop3PorPerfilAsync("moderado"))
            .ReturnsAsync(produtos);

        // Act
        var resultado = await _controller.RecomendadosCliente(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var lista = Assert.IsAssignableFrom<IEnumerable<ProdutoDTO>>(okResult.Value);
        Assert.Equal(2, lista.Count());
        Assert.Contains(lista, p => p.Nome == "Produto A");
    }
}