using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Investimentos.Application.DTOs;

public class SimulacaoRequestDTO
{
    [Required]
    public int ClienteId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    [DefaultValue(0)]
    public double Valor { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "O prazo deve ser maior que zero.")]
    [DefaultValue(0)]
    public int PrazoMeses { get; set; }

    [Required]
    public string TipoProduto { get; set; } = string.Empty;
}