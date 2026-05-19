using System.ComponentModel.DataAnnotations;
using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Models;

public class CreateRequestFormModel
{
    [Required(ErrorMessage = "Выберите вид справки")]
    [Display(Name = "Вид справки")]
    public CertificateType Type { get; set; }

    [Required(ErrorMessage = "Укажите количество экземпляров")]
    [Range(1, 100, ErrorMessage = "Количество должно быть от 1 до 100")]
    [Display(Name = "Количество экземпляров")]
    public int CopiesCount { get; set; } = 1;

    [Required(ErrorMessage = "Укажите причину запроса")]
    [StringLength(1000, ErrorMessage = "Не более 1000 символов")]
    [Display(Name = "Причина запроса")]
    public string Reason { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Не более 200 символов")]
    [Display(Name = "Название справки")]
    public string? CustomTypeName { get; set; }
}
