using cryptobank.api.features.users.domain;

namespace cryptobank.api.features.users.services;

public interface IAccessTokenProvider
{
    Task<string> IssueAsync(User user, CancellationToken cancellationToken);
}