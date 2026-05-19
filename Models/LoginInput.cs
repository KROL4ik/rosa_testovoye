using System.ComponentModel.DataAnnotations;

namespace rosa_testovoye.Models;

public class LoginInput
{
    [Required(ErrorMessage = "Укажите имя пользователя")]
    [Display(Name = "Имя пользователя")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;
}
