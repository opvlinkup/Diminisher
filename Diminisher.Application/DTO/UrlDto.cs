namespace Application.DTO;

public class UrlDto
{
    public Guid Id { get; set; }

    public string? LongUrl { get; set; }

    public string? ShortUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public int Clicks { get; set; }
}