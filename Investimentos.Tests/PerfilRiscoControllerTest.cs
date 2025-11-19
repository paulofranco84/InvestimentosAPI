using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Investimentos.API.Controllers;
using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Investimentos.API.Tests;

public class PerfilRiscoControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUof;
    private readonly Mock<IPerfilRiscoService> _mockPerfilService;
    private readonly PerfilRiscoController _controller;

    public PerfilRiscoControllerTests()
    {
        _mockUof = new Mock<IUnitOfWork>();
        _mockPerfilService = new Mock<IPerfilRiscoService>();
        _controller = new PerfilRiscoController(_mockUof.Object, _mockPerfilService.Object);
    }

    [Fact]
    public async Task ObterPerfil_ClienteNaoEncontrado_RetornaBadRequest()
    {
        // Arrange
        _mockUof.Setup(u => u.ClienteRepository.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync((Cliente)null);

        // Act
        var result = await _controller.ObterPerfil(1);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Cliente não encontrado", badRequestResult.Value);
    }

    [Fact]
    public async Task ObterPerfil_ClienteEncontrado_RetornaOkComPerfil()
    {
        // Arrange
        var cliente = new Cliente { Id = 130, Nome = "Teste" };
        var perfilCalculado = new PerfilRisco
        {
            ClienteId = 130,
            Perfil = "Conservador",
            Pontuacao = 80,
            Descricao = "Perfil conservador com baixa tolerância ao risco"
        };

        _mockUof.Setup(u => u.ClienteRepository.GetAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
                .ReturnsAsync(cliente);

        _mockPerfilService.Setup(s => s.CalcularPerfilAsync(130))
                           .ReturnsAsync(perfilCalculado);

        // Act
        var result = await _controller.ObterPerfil(130);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var retorno = Assert.IsType<PerfilRisco>(okResult.Value);

        Assert.Equal(perfilCalculado.Perfil, retorno.Perfil);
        Assert.Equal(perfilCalculado.ClienteId, retorno.ClienteId);
        Assert.Equal(perfilCalculado.Pontuacao, retorno.Pontuacao);
    }
}