namespace cryptobank.api.features.users.endpoints.getProfile;

public class ProfileModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; } = DateOnly.MinValue;
    public DateTime DateOfRegistration { get; set; } = DateTime.UnixEpoch;
    public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();
}