// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace BooksWishlist.Presentation.Models;

public record AuthTokenResponse
{
    public string token_type { get; set; } = null!;
    public long expires_in { get; set; }
    public string access_token { get; set; } = null!;
}
