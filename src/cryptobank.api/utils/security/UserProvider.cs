using cryptobank.api.features.users.services;

namespace cryptobank.api.utils.security;

[ContainerEntry(ServiceLifetime.Scoped, typeof(IUserProvider))]
internal class UserProvider : IUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int Id => int.Parse(_httpContextAccessor.HttpContext
        ?.User.FindFirst(AccessTokenConstants.ClaimsTypes.Id)
        ?.Value ?? throw new InvalidOperationException("User is not authenticated"));

    public string Email => _httpContextAccessor.HttpContext
        ?.User.FindFirst(AccessTokenConstants.ClaimsTypes.Email)
        ?.Value ?? throw new InvalidOperationException("User is not authenticated");

    public IEnumerable<string> Roles => _httpContextAccessor.HttpContext
        ?.User.FindAll(AccessTokenConstants.ClaimsTypes.Role)
        .Select(claim => claim.Value) ?? throw new InvalidOperationException("User is not authenticated");
}