namespace cryptobank.api.features.users.endpoints.getProfile;

public class GetProfileHandler : IRequestHandler<GetProfileRequest, ProfileModel>
{
    private readonly CryptoBankDbContext _dbContext;

    public GetProfileHandler(CryptoBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProfileModel> Handle(GetProfileRequest request, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.Roles)
            .Where(u => u.Id == request.UserId)
            .Select(u => new ProfileModel
            {
                Id = u.Id,
                Email = u.Email,
                DateOfBirth = u.DateOfBirth,
                DateOfRegistration = u.DateOfRegistration,
                Roles = u.Roles.Select(r => r.Name).ToImmutableList()
            }).SingleAsync(cancellationToken);
    }
}