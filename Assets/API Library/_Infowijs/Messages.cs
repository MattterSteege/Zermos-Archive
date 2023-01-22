#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

public class Messages : BetterHttpClient
{
    [SerializeField] SessionAuthenticatorInfowijs authenticator;
    [SerializeField] private int HardTriesCap = 5;
    
    [ContextMenu("Test")]
    public void test()
    {
        getMessages();
    }

    public InfowijsMessage getMessages()
    {
        InfowijsMessage message = new InfowijsMessage();
        message = DownloadMessages();
        
        while (message.Data.HasMore && HardTriesCap > 0)
        {
            InfowijsMessage tempMessage = DownloadMessages(true, message.Data.Since);
            message.Data.Messages.AddRange(tempMessage.Data.Messages);
            message.Data.Since = tempMessage.Data.Since;
            message.Data.HasMore = tempMessage.Data.HasMore;
            HardTriesCap--;
        }
        
        HardTriesCap = 5;

        return message;
    }
    
    /*
        type catalog:
        1: means message contents
        2: means that that is an attached file (bijlage)     
        3: means that it contains an foto
        
        12: probably means nothing, but is a divider between messages
        
        30: contains information about sender/reader and the title of the post 
    */
    
    public InfowijsMessage DownloadMessages(bool includeArchived = true, long since = 0)
    {
        string baseUrl = $"https://antonius.hoyapp.nl/hoy/v3/messages?include_archived={(includeArchived ? 1 : 0)}&since={since}";
        
        string sessionToken = authenticator.GetSessionToken().data;
        if (sessionToken == null)
        {
            AndroidUIToast.ShowToast("Er ging iets mis tijdens het ophalen van een nieuwe sessie token, probeer het (later) opnieuw.");
            return null;
        }

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", $"Bearer {sessionToken}");
        return (InfowijsMessage) Get(baseUrl, null, headers, (response) =>
        {
            InfowijsMessage infowijsMessage = JsonConvert.DeserializeObject<InfowijsMessage>(response.downloadHandler.text, Converter.Settings);
            return infowijsMessage;
        }, _ =>
        {
            AndroidUIToast.ShowToast("Er ging iets mis tijdens het ophalen van je schoolberichten, probeer het (later) opnieuw.");
            return null;
        });
    }

#region Model
public partial class InfowijsMessage
    {
        [JsonProperty("data")] public Data Data { get; set; } = null!;
        [JsonProperty("meta")] public Meta Meta { get; set; } = null!;
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

    public partial class Meta
    {
        [JsonProperty("statusCode")] public long StatusCode { get; set; }
        [JsonProperty("timestamp")] public long Timestamp { get; set; }
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
#endregion
}
#pragma warning restore CS8632
