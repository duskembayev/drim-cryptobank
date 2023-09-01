namespace cryptobank.api.errors;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, int> ValidUserId<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThan(0)
            .WithErrorCode(GeneralErrorCodes.InvalidUser)
            .WithMessage("User ID must be greater than 0");
    }

    public static IRuleBuilderOptions<T, TEnum> ValidEnumValue<T, TEnum>(this IRuleBuilder<T, TEnum> ruleBuilder)
        where TEnum : struct, Enum
    {
        return ruleBuilder
            .IsInEnum()
            .WithErrorCode(GeneralErrorCodes.InvalidEnumValue)
            .WithMessage($"{{PropertyName}} must be a valid {typeof(TEnum).Name} value");
    }
}