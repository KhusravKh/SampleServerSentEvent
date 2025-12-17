using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SampleServerSentEvent.Models;

public class Category
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MinLength(3,  ErrorMessage = "Category name must be at least 3 characters long")]
    [MaxLength(30,  ErrorMessage = "Category name cannot be more than 30 characters long")]
    public string Name { get; set; }

    [Required]
    [DisplayName("Display Order")]
    [Range(1, 100, ErrorMessage = "Display Order must be between 1 and 100")]
    public long DisplayOrder { get; set; }
}