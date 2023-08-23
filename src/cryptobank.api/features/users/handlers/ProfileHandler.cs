namespace cryptobank.api.features.users.handlers;

public class ProfileHandler : IRequestHandler<ProfileRequest, ProfileResponse>
{
    private readonly CryptoBankDbContext _dbContext;

    public ProfileHandler(CryptoBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProfileResponse> Handle(ProfileRequest request, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.Roles)
            .Where(u => u.Id == request.UserId)
            .Select(u => new ProfileResponse
            {
                Id = u.Id,
                Email = u.Email,
                DateOfBirth = u.DateOfBirth,
                DateOfRegistration = u.DateOfRegistration,
                Roles = u.Roles.Select(r => r.Name).ToList()
            }).FirstAsync(cancellationToken);
    }
}