using BooksWishlist.Application.Users.Entities;

namespace BooksWishlist.Application.Users;

public class UserCommands : IRepository<User>
{
    public Task<User> Create(User entity) => throw new NotImplementedException();

    public Task<User> Delete(User entity) => throw new NotImplementedException();

    public Task<IEnumerable<User>> Read() => throw new NotImplementedException();

    public Task<User> Update(User entity) => throw new NotImplementedException();
}
