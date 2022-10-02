using System.Linq.Expressions;
using System.Text.RegularExpressions;
using MongoDB.Bson;

namespace BooksWishlist.Infrastructure.Extensions;

public static class MongoExtensions
{
    public static FilterDefinition<TDocument> In<TDocument>(
        this FilterDefinitionBuilder<TDocument> builder,
        Expression<Func<TDocument, object>> expr,
        IEnumerable<string> values,
        bool ignoreCase)
    {
        if (!ignoreCase)
        {
            return builder.In(expr, values);
        }

        var filters = values
            .Select(v => builder.Regex(expr, new BsonRegularExpression($"^{Regex.Escape(v)}$", "i")));
        return builder.Or(filters);
    }

    public static FilterDefinition<TDocument> EqCase<TDocument>(
        this FilterDefinitionBuilder<TDocument> builder,
        Expression<Func<TDocument, object>> expr,
        string value,
        bool ignoreCase) =>
        !ignoreCase
            ? builder.Eq(expr, value)
            : builder.Regex(expr, new BsonRegularExpression($"^{Regex.Escape(value)}$", "i"));
}
