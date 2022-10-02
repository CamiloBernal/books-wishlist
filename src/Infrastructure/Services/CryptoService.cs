using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BooksWishlist.Infrastructure.Services;

internal class CryptoService
{
    private readonly string _saltValue;

    public CryptoService(IOptions<CryptoServiceSettings> settings) => _saltValue = settings.Value.SaltValue;

    private byte[] GetSaltByteArray() => new UTF8Encoding().GetBytes(_saltValue);


    public string EncryptString(string value) =>
        Convert.ToBase64String(KeyDerivation.Pbkdf2(
            value,
            GetSaltByteArray(),
            KeyDerivationPrf.HMACSHA256,
            100000,
            256 / 8));
}
