namespace BooksWishlist.Infrastructure.Services.GoogleBooksResponseModels;

public class GoogleBooksSearchResultDto
{
    public int? TotalItems { get; set; } = 0;
    public IEnumerable<BookInfoDto>? Books { get; set; }

    public static implicit operator GoogleBooksSearchResultDto(GoogleBooksSearchResults results)
    {
        var dto = new GoogleBooksSearchResultDto { TotalItems = results.TotalItems, Books = results?.Items?.Select(i => new BookInfoDto
            {
                Id = i.Id,
                SelfLink = i.SelfLink,
                Title = i.VolumeInfo?.Title,
                Authors = i.VolumeInfo?.Authors,
                Publisher = i.VolumeInfo?.Publisher,
                Description = i.VolumeInfo?.Publisher,
                ContentVersion = i.VolumeInfo?.ContentVersion,
                PreviewLink = i.VolumeInfo?.PreviewLink,
                InfoLink = i.VolumeInfo?.InfoLink,
                Thumbnail = i.VolumeInfo?.ImageLinks?.Thumbnail,
                PublishedDate = i.VolumeInfo?.PublishedDate
            })
        };
        return dto;
    }
}
