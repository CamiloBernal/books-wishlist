using System.ComponentModel.DataAnnotations;
using BooksWishlist.Application.Common;

namespace BooksWishlist.Application.Users.Entities;

public class User
{
    [Key] [OpenApiIgnoreMember] public Guid UserId { get; set; } = Guid.NewGuid();
    public string UserName { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
