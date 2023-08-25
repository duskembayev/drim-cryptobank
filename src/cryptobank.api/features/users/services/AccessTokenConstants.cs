namespace cryptobank.api.features.users.services;

public static class AccessTokenConstants
{
    public const string Bearer = "Bearer";

    public static class ClaimsTypes
    {
        public const string Id = "id";
        public const string Email = "email";
        public const string Role = "roles";
    }
}