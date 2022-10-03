namespace BooksWishlist.Infrastructure.Store;

internal class StringObjectIdConvention : ConventionBase, IPostProcessingConvention
{
    public void PostProcess(BsonClassMap classMap)
    {
        var idMap = classMap.IdMemberMap;
        if (idMap is { MemberName: "Id" } && idMap.MemberType == typeof(string))
        {
            idMap.SetIdGenerator(new StringObjectIdGenerator());
        }
    }
}
