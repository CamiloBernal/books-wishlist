using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;

namespace BooksWishlist.Infrastructure.Databases;

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
