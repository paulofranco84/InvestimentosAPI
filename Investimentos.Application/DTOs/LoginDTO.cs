using System.ComponentModel.DataAnnotations;

namespace Investimentos.Application.DTOs;

public class LoginDTO
{
    [Required(ErrorMessage = "O nome do usuário é obrigatório")]
    public string? Username { get; set; }
    
    [Required(ErrorMessage = "A senha é obrigatória")]
    public string? Password { get; set; }
}
