namespace BooksWishlist.Application.Users;

using Entities;

public class UserCommands : IRepository<User>
{
    public Task<User> Create(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<User> Delete(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> Read()
    {
        throw new NotImplementedException();
    }

    public Task<User> Update(User entity)
    {
        throw new NotImplementedException();
    }
}
