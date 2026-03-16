using System.Security.Cryptography;
using Application.Abstractions.Generators;

namespace Application.Services;

public sealed class ShortCodeGenerator : IShortCodeGenerator
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private const int AlphabetLength = 62;
    private const int Mask = 63;

    private const int Length = 10;

    public string Generate()
    {
        Span<char> result = stackalloc char[Length];
        Span<byte> randomByteBuffer = stackalloc byte[Length * 2];

        var currentIndex = 0;

        while (currentIndex < Length)
        {
            RandomNumberGenerator.Fill(randomByteBuffer);

            for (var i = 0; i < randomByteBuffer.Length && currentIndex < Length; i++)
            {
                var value = randomByteBuffer[i] & Mask;

                if (value >= AlphabetLength)
                    continue;

                result[currentIndex++] = Alphabet[value];
            }
        }

        return new string(result);
    }
}