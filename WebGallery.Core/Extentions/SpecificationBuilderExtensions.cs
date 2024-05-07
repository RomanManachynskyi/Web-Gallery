using Ardalis.Specification;
using System.Linq.Expressions;
using WebGallery.Core.Dtos;
using WebGallery.Core.Exceptions;

namespace WebGallery.Core.Extentions;

public static class SpecificationBuilderExtensions
{
    public static void ApplyListRequest<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        SortParameters sortParameters,
        PaginationParameters paginationParameters)
    {
        if (sortParameters is not null)
            specificationBuilder.OrderByColumnName(sortParameters.Sort, sortParameters.OrderByAscending);

        specificationBuilder.Skip(paginationParameters.Offset).Take(paginationParameters.Limit);
    }

    private static void OrderByColumnName<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        string columnName,
        bool isOrderAscending)
    {
        var matchedProperty = typeof(T).GetProperties().FirstOrDefault(p => p.Name == columnName)
            ?? throw new SpecificationBuilderException(
                $"Invalid column name: {columnName}!",
                $"Type {typeof(T).Name} does not contain a property with name {columnName}");
        var paramExpr = Expression.Parameter(typeof(T));
        var propAccess = Expression.Property(paramExpr, matchedProperty.Name);
        var expr = Expression.Lambda<Func<T, object?>>(Expression.Convert(propAccess, typeof(object)), paramExpr);

        (specificationBuilder.Specification.OrderExpressions as List<OrderExpressionInfo<T>>)
            ?.Add(new OrderExpressionInfo<T>(
                expr,
                isOrderAscending ? OrderTypeEnum.OrderBy : OrderTypeEnum.OrderByDescending));
    }
}
