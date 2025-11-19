using System.ComponentModel.DataAnnotations;

namespace Investimentos.Application.DTOs;

public class RegisterDTO
{
    [Required(ErrorMessage = "O nome do usuário é obrigatório")]
    public string? Username { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "O Email é obrigatório")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória")]
    public string? Password { get; set; }
}
