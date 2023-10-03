namespace cryptobank.api.tests.features.accounts;

[CollectionDefinition(Name)]
public class AccountsCollection : ICollectionFixture<ApplicationFixture>
{
    public const string Name = nameof(AccountsCollection);
}