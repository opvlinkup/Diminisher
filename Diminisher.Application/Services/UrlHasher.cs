using System.Security.Cryptography;
using System.Text;
using Application.Abstractions;

namespace Application.Services;

public class UrlHasher : IUrlHasher
{
    public byte[] ComputeHash(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        var bytes = Encoding.UTF8.GetBytes(url);

        return SHA256.HashData(bytes);
    }
}