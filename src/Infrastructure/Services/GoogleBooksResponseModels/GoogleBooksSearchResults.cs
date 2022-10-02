// ReSharper disable ClassNeverInstantiated.Global
namespace BooksWishlist.Infrastructure.Services.GoogleBooksResponseModels;

public class GoogleBooksSearchResults
{
    public string? Kind { get; set; }
    public int? TotalItems { get; set; } = 0;
    public IEnumerable<BookItem>? Items { get; set; }
}

public class BookItem
{
    public string? Kind { get; set; }
    public string? Id { get; set; }
    public string? Etag { get; set; }
    public string? SelfLink { get; set; }
    public VolumeInfo? VolumeInfo { get; set; }
    //public SaleInfo? SaleInfo { get; set; }
    //public AccessInfo? AccessInfo { get; set; }
    //public SearchInfo? SearchInfo { get; set; }
}

public class VolumeInfo
{
    public string Title { get; set; }
    public List<string> Authors { get; set; }
    public string Publisher { get; set; }
    public string PublishedDate { get; set; }
    public string Description { get; set; }
    //public ReadingModes ReadingModes { get; set; }
    public string MaturityRating { get; set; }
    public bool AllowAnonLogging { get; set; }
    public string ContentVersion { get; set; }
    //public PanelizationSummary PanelizationSummary { get; set; }
    //public ImageLinks ImageLinks { get; set; }
    public string PreviewLink { get; set; }
    public string InfoLink { get; set; }
    public string CanonicalVolumeLink { get; set; }
    public string Subtitle { get; set; }
}
//
// public class AccessInfo
// {
//     public string Country { get; set; }
//     public Epub Epub { get; set; }
//     public Pdf Pdf { get; set; }
//     public string AccessViewStatus { get; set; }
// }
//
// public class Epub
// {
//     public bool IsAvailable { get; set; }
//     public string AcsTokenLink { get; set; }
// }
//
// public class ImageLinks
// {
//     public string SmallThumbnail { get; set; }
//     public string Thumbnail { get; set; }
// }
//
// public class ListPrice
// {
//     public int Amount { get; set; }
//     public string CurrencyCode { get; set; }
//     public object AmountInMicros { get; set; }
// }
//
// public class Offer
// {
//     public int FinskyOfferType { get; set; }
//     public ListPrice ListPrice { get; set; }
//     public RetailPrice RetailPrice { get; set; }
// }
//
// public class PanelizationSummary
// {
//     public bool ContainsEpubBubbles { get; set; }
//     public bool ContainsImageBubbles { get; set; }
// }
//
// public class Pdf
// {
//     public bool IsAvailable { get; set; }
//     public string AcsTokenLink { get; set; }
// }
//
// public class ReadingModes
// {
//     public bool Text { get; set; }
//     public bool Image { get; set; }
// }
//
// public class RetailPrice
// {
//     public int Amount { get; set; }
//     public string CurrencyCode { get; set; }
//     public object AmountInMicros { get; set; }
// }
//
// public class SaleInfo
// {
//     public string Country { get; set; }
//     public ListPrice ListPrice { get; set; }
//     public RetailPrice RetailPrice { get; set; }
//     public string BuyLink { get; set; }
//     public List<Offer> Offers { get; set; }
// }
//
// public class SearchInfo
// {
//     public string TextSnippet { get; set; }
// }
//
//
