using BooksWishlist.Application.UserWishlists.Entities;

namespace BooksWishlist.Presentation.ViewModels;

public class WishlistDto
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public IEnumerable<string>? Tags { get; set; } = new List<string>();

    public IEnumerable<string>? Books { get; set; } = new List<string>();

    public static implicit operator UserWishlists(WishlistDto dto)
    {
        var userWishlist = new UserWishlists
        {
            Name = dto.Name, Description = dto.Description, Notes = dto.Notes, Tags = dto.Tags
        };
        return userWishlist;
    }
}
