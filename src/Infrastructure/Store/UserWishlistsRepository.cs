﻿using BooksWishlist.Application.Books.Entities;
using BooksWishlist.Application.Extensions;

// ReSharper disable PossibleMultipleEnumeration

namespace BooksWishlist.Infrastructure.Store;

public class UserWishlistsRepository : IUserWishlistsRepository
{
    private readonly ILoggerService _log;
    private readonly BooksWishlistUnitOfWork<UserWishlists> _unitOfWork;

    public UserWishlistsRepository(ILoggerService log, IOptions<StoreDatabaseSettings> storeSettings)
    {
        _unitOfWork = new BooksWishlistUnitOfWork<UserWishlists>(storeSettings, log, "UserWishlists");
        _log = log;
    }

    public async Task<UserWishlists> CreateAsync(UserWishlists list, CancellationToken cancellationToken = default)
    {
        var listExists = await WishListExists(list.Name, list.OwnerId, cancellationToken);
        if (listExists)
        {
            throw new DuplicateEntityException($"The list with name: {list.Name} already exists in the database.");
        }

        await _unitOfWork.CreateAsync(list, cancellationToken);
        return list;
    }

    public async Task<UserWishlists> UpdateAsync(UserWishlists list, string listName, string owner,
        CancellationToken cancellationToken = default)
    {
        var foundList = await GetUserWishlist(listName, owner, cancellationToken: cancellationToken);
        foundList.list.Merge(list);
        await _unitOfWork.UpdateAsync(foundList.filterDefinition, list, cancellationToken);
        return foundList.list;
    }

    public async Task<bool> DeleteAsync(string listName, string owner, CancellationToken cancellationToken = default)
    {
        var filterDefinition =
            await GetUserWishlist(listName, owner,
                cancellationToken: cancellationToken); //For handle not found exception
        await _unitOfWork.RemoveAsync(filterDefinition.filterDefinition, cancellationToken);
        _log.LogWarning($"The Wishlist with name {listName} associated with the user {owner} and was deleted.");
        return true;
    }

    public async Task<bool?> AddBooksAsync(string listName, IEnumerable<Book?>? books, string owner,
        CancellationToken cancellationToken = default)
    {
        if (books is null) return null;
        var foundList = await GetUserWishlist(listName, owner, cancellationToken: cancellationToken);
        if (foundList.list.Books is not null && foundList.list.Books.Any())
        {
            var conflictedBooks = (from registeredBooks in foundList.list.Books
                join newBooks in books on registeredBooks.BookId equals newBooks.BookId
                select newBooks).ToList();
            if (conflictedBooks.Any())
            {
                throw new DuplicatedBookInListException($"The book had already been added to the WishList {listName}");
            }

            foundList.list.Books = foundList.list.Books.Concat(books);
        }
        else
        {
            foundList.list.Books = books;
        }

        await _unitOfWork.UpdateAsync(foundList.filterDefinition, foundList.list, cancellationToken);
        return true;
    }

    public async Task<bool?> RemoveBooksAsync(string listName, string bookId, string owner,
        CancellationToken cancellationToken = default)
    {
        var foundList = await GetUserWishlist(listName, owner, cancellationToken: cancellationToken);
        var foundBook = foundList.list.Books?.FirstOrDefault(b => b?.BookId == bookId);
        if (foundBook is null) throw new BookNotFoundInListException();
        foundList.list.Books = foundList.list.Books?.Where(b => b?.BookId != bookId);
        await _unitOfWork.UpdateAsync(foundList.filterDefinition, foundList.list, cancellationToken);
        return true;
    }

    public async Task<IEnumerable<Book?>?> GetListBooks(string listName, string owner,
        CancellationToken cancellationToken = default)
    {
        var foundList = await GetUserWishlist(listName, owner, cancellationToken: cancellationToken);
        return foundList.list?.Books;
    }

    public Task<UserWishlists?> FindByNameAsync(string listName, string owner,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();


    public async Task<IEnumerable<UserWishlists?>> FindByOwnerAsync(string owner,
        CancellationToken cancellationToken = default)
    {
        var filterDefinition = new FilterDefinitionBuilder<UserWishlists>().Eq(l => l.OwnerId, owner);
        return await _unitOfWork.GetAsync(filterDefinition, cancellationToken);
    }

    private static FilterDefinition<UserWishlists> GetFilterByListName(string name) =>
        Builders<UserWishlists>.Filter.EqCase(list => list.Name, name, true);

    private static FilterDefinition<UserWishlists> GetFilterByNameAndOwner(string listName, string owner)
    {
        var filterByName = GetFilterByListName(listName);
        var filterDefinition = new FilterDefinitionBuilder<UserWishlists>().And(filterByName,
            new FilterDefinitionBuilder<UserWishlists>().Eq(l => l.OwnerId, owner));
        return filterDefinition;
    }


    private async Task<(UserWishlists list, FilterDefinition<UserWishlists> filterDefinition)> GetUserWishlist(
        string listName, string owner, bool handleNotFoundException = true,
        CancellationToken cancellationToken = default)
    {
        var filterDefinition = GetFilterByNameAndOwner(listName, owner);
        var foundList = await _unitOfWork.GetOneAsync(filterDefinition, cancellationToken);
        if (foundList == null && handleNotFoundException) throw new WishListNotFoundException();
        return (foundList, filterDefinition);
    }


    private async Task<bool> WishListExists(string listName, string owner,
        CancellationToken cancellationToken = default)
    {
        var filterDefinition = GetFilterByNameAndOwner(listName, owner);
        var count = await _unitOfWork.CountAsync(filterDefinition, cancellationToken);
        return count > 0;
    }
}
