using System.ComponentModel.DataAnnotations;
using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Models;

public class RegisterInput
{
    [Required(ErrorMessage = "Укажите имя пользователя")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "От 3 до 100 символов")]
    [Display(Name = "Имя пользователя")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите ФИО")]
    [StringLength(200, ErrorMessage = "Не более 200 символов")]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Не более 200 символов")]
    [Display(Name = "Отдел")]
    public string? Department { get; set; }

    [Required(ErrorMessage = "Выберите роль")]
    [Display(Name = "Роль")]
    public UserRole Role { get; set; }

    [Required(ErrorMessage = "Укажите пароль")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Не менее 6 символов")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Повторите пароль")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    [Display(Name = "Подтверждение пароля")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
