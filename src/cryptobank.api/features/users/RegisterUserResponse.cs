using cryptobank.dal.users;

namespace cryptobank.api.features.users;

public class RegisterUserResponse
{
    public int UserId { get; set; }
    public RoleId Role { get; set; }
}