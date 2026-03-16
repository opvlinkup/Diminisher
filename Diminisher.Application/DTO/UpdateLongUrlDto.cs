using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class UpdateLongUrlDto
{
        [Required]
        [Url]
        [MaxLength(2048)]
        public string LongUrl { get; init; } = null!;
}