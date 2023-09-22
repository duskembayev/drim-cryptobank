using System.Linq.Expressions;
using cryptobank.api.features.accounts.domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace cryptobank.api.db;

internal static class EntityTypeBuilderExtensions
{
    public static PropertyBuilder<Currency> CurrencyProperty<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, Currency>> propertyExpression,
        bool required = true) where TEntity : class
    {
        return builder
            .Property(propertyExpression)
            .HasConversion<EnumToStringConverter<Currency>>()
            .IsRequired(required)
            .HasMaxLength(3);
    }

    public static PropertyBuilder<decimal> MoneyProperty<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, decimal>> propertyExpression,
        bool required = true) where TEntity : class
    {
        return builder
            .Property(propertyExpression)
            .IsRequired(required)
            .HasPrecision(32, 16);
    }
}