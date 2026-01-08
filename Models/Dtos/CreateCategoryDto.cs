using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "The Name field is required.")]
    [MaxLength(50, ErrorMessage = "The Name field must not exceed 50 characters.")]
    [MinLength(3, ErrorMessage = "The Name field must be at least 3 characters.")]
    public string Name { get; set; } = string.Empty;
}
