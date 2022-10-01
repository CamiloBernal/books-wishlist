using System.ComponentModel.DataAnnotations;
using BooksWishlist.Application.Common;

namespace BooksWishlist.Application.Users.Entities;

public class User
{
    [Key] [OpenApiIgnoreMember] public Guid UserId { get; set; } = Guid.NewGuid();
    public string UserName { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; }
}
