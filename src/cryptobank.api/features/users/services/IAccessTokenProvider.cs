using cryptobank.api.features.users.domain;

namespace cryptobank.api.features.users.services;

public interface IAccessTokenProvider
{
    string Issue(User user);
}