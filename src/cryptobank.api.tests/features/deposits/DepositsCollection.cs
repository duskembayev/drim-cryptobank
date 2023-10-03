namespace cryptobank.api.tests.features.deposits;

[CollectionDefinition(Name)]
public class DepositsCollection : ICollectionFixture<ApplicationFixture>
{
    public const string Name = nameof(DepositsCollection);
}