namespace BooksWishlist.Application.Books.Entities;

public class Book
{

    public string BookId { get; set; }

    public IEnumerable<string> Authors { get; set; } = new List<string>();

    public string Title { get; set; }

    public string Publisher { get; set; }

}
