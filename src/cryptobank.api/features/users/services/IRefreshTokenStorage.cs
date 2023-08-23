namespace cryptobank.api.features.users.services;

public interface IRefreshTokenStorage
{
    string Issue(int userId, bool allowExtend);
    (int UserId, string Token)? Renew(string token);
    void Revoke(string token);
    void RevokeAll(int userId);
}