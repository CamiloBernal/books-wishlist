using BooksWishlist.Application.Books.Entities;

namespace BooksWishlist.Application.UserWishlists.Entities;

public class UserWishlists
{
    public Guid Owner { get; set; }

    public string Name { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public IEnumerable<string> Tags { get; set; } = new List<string>();

    public IEnumerable<Book> Books { get; set; } = new List<Book>();
}
