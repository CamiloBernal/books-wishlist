using System.Text;
using BooksWishlist.Infrastructure.Settings;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;

namespace BooksWishlist.Infrastructure.Services;

internal class CryptoService
{
    private readonly string _saltValue;

    public CryptoService(IOptions<CryptoServiceSettings> settings)
    {
        _saltValue = settings.Value.SaltValue;
    }

    private byte[] GetSaltByteArray()
    {
        return new UTF8Encoding().GetBytes(_saltValue);
    }


    public string EncryptString(string value)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            value,
            GetSaltByteArray(),
            KeyDerivationPrf.HMACSHA256,
            100000,
            256 / 8));
    }
}
