namespace Domain.Models;

public class ShortUrl
{
    public Guid Id { get; set; }

    public string LongUrl { get; set; } = null!;
    
    public byte[] LongUrlHash { get; set; } = null!;

    public string Code { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int Clicks { get; set; }
}