using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zermos_Web.Models
{
    public class InfowijsMessagesModel
    {
        [JsonProperty("data")] public Data Data { get; set; } = null!;
    }

    public partial class Data
    {
        [JsonProperty("messages")] public List<Message> Messages { get; set; } = null!;
        [JsonProperty("since")] public long Since { get; set; }
        [JsonProperty("hasMore")] public bool HasMore { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("id")] public Guid Id { get; set; }
        [JsonProperty("type")] public long Type { get; set; }
        [JsonProperty("content")] public ContentUnion Content { get; set; }
        [JsonProperty("createdBy")] public long CreatedBy { get; set; }
        [JsonProperty("createdAt")] public long CreatedAt { get; set; }
        [JsonProperty("broadcast")] public bool Broadcast { get; set; }
        [JsonProperty("receiver")] public long Receiver { get; set; }
        [JsonProperty("groupId")] public Guid GroupId { get; set; }
        [JsonProperty("isMe")] public bool IsMe { get; set; }
    }

    public partial class ContentClass
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)] public Guid? Id { get; set; }
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)] public string Title { get; set; }
        [JsonProperty("groupTitle", NullValueHandling = NullValueHandling.Ignore)] public string? GroupTitle { get; set; }
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)] public long? Type { get; set; }
        [JsonProperty("users", NullValueHandling = NullValueHandling.Ignore)] public User[] Users { get; set; }
        [JsonProperty("createdAt", NullValueHandling = NullValueHandling.Ignore)] public long? CreatedAt { get; set; }
        [JsonProperty("updatedAt", NullValueHandling = NullValueHandling.Ignore)] public long? UpdatedAt { get; set; }
        [JsonProperty("closedAt", NullValueHandling = NullValueHandling.Ignore)] public long? ClosedAt { get; set; }
        [JsonProperty("createdBy", NullValueHandling = NullValueHandling.Ignore)] public long? CreatedBy { get; set; }
        [JsonProperty("createdByMe", NullValueHandling = NullValueHandling.Ignore)] public bool? CreatedByMe { get; set; }
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)] public long? Timestamp { get; set; }
        [JsonProperty("image")] public object Image { get; set; }
        [JsonProperty("sender", NullValueHandling = NullValueHandling.Ignore)] public long? Sender { get; set; }
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)] public string Name { get; set; }
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)] public Uri Url { get; set; }
    }

    public partial class User
    {
        [JsonProperty("id")] public long Id { get; set; }
        [JsonProperty("isMe")] public bool IsMe { get; set; }
        [JsonProperty("avatar")] public Uri Avatar { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("subtitle", NullValueHandling = NullValueHandling.Ignore)] public string? Subtitle { get; set; }
        [JsonProperty("firstName", NullValueHandling = NullValueHandling.Ignore)] public string? FirstName { get; set; }
        [JsonProperty("lastName", NullValueHandling = NullValueHandling.Ignore)] public string? LastName { get; set; }
        [JsonProperty("role")] public string Role { get; set; }
        [JsonProperty("conversationRole")] public string ConversationRole { get; set; }
        [JsonProperty("deleted_at")] public long DeletedAt { get; set; }
    }

    public partial struct ContentUnion
    {
        public ContentClass ContentClass;
        public string String;

        public static implicit operator ContentUnion(ContentClass ContentClass) => new ContentUnion { ContentClass = ContentClass };
        public static implicit operator ContentUnion(string String) => new ContentUnion { String = String };
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ContentUnionConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
    
    internal class ContentUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ContentUnion) || t == typeof(ContentUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new ContentUnion { String = stringValue };
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<ContentClass>(reader);
                    return new ContentUnion { ContentClass = objectValue };
            }
            throw new Exception("Cannot unmarshal type ContentUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ContentUnion)untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            if (value.ContentClass != null)
            {
                serializer.Serialize(writer, value.ContentClass);
                return;
            }
            throw new Exception("Cannot marshal type ContentUnion");
        }

        public static readonly ContentUnionConverter Singleton = new ContentUnionConverter();
    }
}