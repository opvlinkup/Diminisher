namespace Application.Abstractions;


public interface IUrlHasher
{
    byte[] ComputeHash(string url);
}