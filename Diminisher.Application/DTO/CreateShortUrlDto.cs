using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class CreateShortUrlDto
{
    [Required]
    [Url]
    [MaxLength(2048)]
    public string? LongUrl { get; set; }
}