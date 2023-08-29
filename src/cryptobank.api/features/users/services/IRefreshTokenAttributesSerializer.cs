namespace cryptobank.api.features.users.services;

internal interface IRefreshTokenAttributesSerializer
{
    string Serialize(RefreshTokenAttributes attributes);
    RefreshTokenAttributes Deserialize(string value);

    internal readonly record struct RefreshTokenAttributes(int UserId, bool AllowExtend)
    {
        public bool IsRevoked { get; init; } = false;
        public string? ReplacedBy { get; init; } = null;
    }
}