namespace BooksWishlist.Presentation.Configuration;

public class TokenGeneratorOptions
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SigningKey { get; set; } = null!;
    public long MinutesToExpire { get; set; } = 60;
}
