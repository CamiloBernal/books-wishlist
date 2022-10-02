using System.ComponentModel.DataAnnotations;
using BooksWishlist.Application.Common;

namespace BooksWishlist.Application.Users.Entities;

public class User
{
    [OpenApiIgnoreMember] public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserName { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
