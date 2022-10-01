using System.ComponentModel.DataAnnotations;
using BooksWishlist.Application.Common;

namespace BooksWishlist.Application.Users.Entities;

public class User
{
    [OpenApiIgnoreMember] public Guid Id { get; set; } = Guid.NewGuid();
    public string UserName { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
