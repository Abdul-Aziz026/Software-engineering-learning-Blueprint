using Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Infrastructure.Persistence.Serializers;

/// <summary>
/// Tells the MongoDB driver how to read/write an <see cref="Email"/> value object.
/// We store it as a plain BSON string (not a nested { "Value": "..." } document),
/// so the on-disk schema is identical to the old `string Email` field — no data
/// migration is needed. On the way back in, <see cref="Email.Create"/> re-validates,
/// guaranteeing the Domain never holds an invalid Email even if the DB was tampered with.
/// </summary>
public sealed class EmailSerializer : SerializerBase<Email>
{
    public override void Serialize(
        BsonSerializationContext context, BsonSerializationArgs args, Email value)
    {
        if (value is null)
        {
            context.Writer.WriteNull();
        }
        else
        {
            context.Writer.WriteString(value.Value);
        }
    }

    public override Email Deserialize(
        BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;
        if (reader.CurrentBsonType == BsonType.Null)
        {
            reader.ReadNull();
            return null!;
        }

        return Email.Create(reader.ReadString());
    }
}
