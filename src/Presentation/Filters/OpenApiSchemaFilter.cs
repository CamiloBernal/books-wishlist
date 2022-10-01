namespace BooksWishlist.Presentation.Filters;

public class OpenApiSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null)
        {
            return;
        }

        var ignoreDataMemberProperties = context.Type.GetProperties()
            .Where(t => t.GetCustomAttribute<OpenApiIgnoreMemberAttribute>() != null);
        foreach (var ignoreDataMemberProperty in ignoreDataMemberProperties)
        {
            var propertyToHide = schema.Properties.Keys
                .SingleOrDefault(x =>
                    string.Equals(x, ignoreDataMemberProperty.Name, StringComparison.CurrentCultureIgnoreCase));
            if (propertyToHide != null)
            {
                schema.Properties.Remove(propertyToHide);
            }
        }
    }
}
