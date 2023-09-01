using System.Text.Json.Nodes;
using Attr = cryptobank.api.features.users.services.IRefreshTokenAttributesSerializer.RefreshTokenAttributes;


namespace cryptobank.api.features.users.services;

[ContainerEntry(ServiceLifetime.Singleton, typeof(IRefreshTokenAttributesSerializer))]
internal class RefreshTokenAttributesSerializer : IRefreshTokenAttributesSerializer
{
    private const string AttrIdField = "id";
    private const string AttrExtendField = "ed";
    private const string AttrReplacedByField = "by";
    private const string AttrRevokedField = "rv";

    public string Serialize(Attr attributes)
    {
        var jsonNode = new JsonObject
        {
            { AttrIdField, attributes.UserId },
            { AttrExtendField, attributes.AllowExtend },
            { AttrReplacedByField, attributes.ReplacedBy },
            { AttrRevokedField, attributes.IsRevoked }
        };

        return jsonNode.ToJsonString();
    }

    public Attr Deserialize(string value)
    {
        var jsonNode = JsonNode.Parse(value) ?? throw new FormatException("Failed to parse token attributes");

        var userId = jsonNode[AttrIdField]?.GetValue<int?>();
        var extend = jsonNode[AttrExtendField]?.GetValue<bool?>();
        var replacedBy = jsonNode[AttrReplacedByField]?.GetValue<string?>();
        var revoked = jsonNode[AttrRevokedField]?.GetValue<bool?>();

        if (!userId.HasValue || !extend.HasValue || !revoked.HasValue)
            throw new FormatException("Invalid token attributes format");

        return new Attr(userId.Value, extend.Value)
        {
            IsRevoked = revoked.Value,
            ReplacedBy = replacedBy
        };
    }
}