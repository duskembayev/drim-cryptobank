namespace cryptobank.api.tests.features.users;

[CollectionDefinition(Name)]
public class UsersCollection : ICollectionFixture<ApplicationFixture>
{
    public const string Name = nameof(UsersCollection);
}