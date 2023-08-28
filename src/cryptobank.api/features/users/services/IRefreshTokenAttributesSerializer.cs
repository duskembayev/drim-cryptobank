namespace cryptobank.api.features.users.services;

internal interface IRefreshTokenAttributesSerializer
{
    internal readonly record struct RefreshTokenAttributes(int UserId, bool AllowExtend)
    {
        public bool IsRevoked { get; init; } = false;
        public string? ReplacedBy { get; init; } = null;
    }

    string Serialize(RefreshTokenAttributes attributes);
    RefreshTokenAttributes Deserialize(string value);
}