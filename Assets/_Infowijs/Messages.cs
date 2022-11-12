using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Messages : MonoBehaviour
{
    [SerializeField] AuthenticateInfowijs authenticateInfowijs;
    
    [ContextMenu("Test")]
    public void Test()
    {
        GetBetterInfowijsMessages();
    }
    
    public RawInfowijsMessages GetInfowijsMessages(bool includeArchived = true, int since = 0)
    {
        string access_token = authenticateInfowijs.GetAccesToken().data;
        
        var baseurl = string.Format($"https://antonius.hoyapp.nl/hoy/v3/messages?include_archived={(includeArchived ? "1" : "0")}&since={since}");

        var www = UnityWebRequest.Get(baseurl);
        www.SetRequestHeader("authorization", "Bearer " + access_token);
        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        var response = JsonConvert.DeserializeObject<RawInfowijsMessages>(www.downloadHandler.text);

        www.Dispose();

        int maxtries = 10;
        while (response.data.hasMore && maxtries > 0)
        {
            maxtries--;
            var moreMessages = GetInfowijsMessages(includeArchived, response.data.since);
            response.data.messages.AddRange(moreMessages.data.messages);
            response.data.since = moreMessages.data.since;
            response.data.hasMore = moreMessages.data.hasMore;
        }

        return response;
    }

    /*
        type catalog:
        1: means message contents
        2: means that that is an attached file (bijlage)     
        3: means that it contains an foto

        12: no clue it said '1' i just filter and remove them

        30: contains information about sender/reader and the title of the post 
     */
    
    public List<Message> GetBetterInfowijsMessages()
    {
        var messages = GetInfowijsMessages();
        messages.data.messages.RemoveAll(x => x.type == 2); // TODO: add support for attachments
        messages.data.messages.RemoveAll(x => x.type == 3); // TODO: add support for foto's
        messages.data.messages.RemoveAll(x => x.type == 12);
        messages.data.messages.RemoveAll(x => x.type == 30); // TODO: add support for titles
        
        List<Message> betterMessages = new List<Message>();

        
        for (int i = 0; i < messages.data.messages.Count / 2; i++)
        {
            try
            {
                //string title = JsonConvert.DeserializeObject<RawContent>(messages.data.messages[i * 2].content.ToString())?.title ?? "Error getting title";

                betterMessages.Add(new Message
                {
                    id = i.ToString(),
                    type = messages.data.messages[i * 2 + 1].type,
                    title = "Error getting title",
                    content = messages.data.messages[i * 2 + 1].content.ToString(),
                    createdBy = messages.data.messages[i * 2 + 1].createdBy,
                    createdAt = messages.data.messages[i * 2 + 1].createdAt,
                    broadcast = messages.data.messages[i * 2 + 1].broadcast,
                    receiver = messages.data.messages[i * 2 + 1].receiver,
                    groupId = messages.data.messages[i * 2 + 1].groupId,
                    isMe = messages.data.messages[i * 2 + 1].isMe,
                });
            }
            catch (Exception) { }
        }
        
        //i * 2 because we want to skip every other message
        //i * 2 + 1

        return betterMessages;
    }
}

#region models - Raw
public class RawData
{
    public List<RawMessage> messages { get; set; }
    public int since { get; set; }
    public bool hasMore { get; set; }
}

public class RawMessage
{
    public string id { get; set; }
    public int type { get; set; }
    public object content { get; set; }
    public object createdBy { get; set; }
    public int createdAt { get; set; }
    public bool broadcast { get; set; }
    public object receiver { get; set; }
    public string groupId { get; set; }
    public bool isMe { get; set; }
}

public class RawContent
{
    public string id { get; set; }
    public string title { get; set; }
    public string groupTitle { get; set; }
    public int type { get; set; }
    public int createdAt { get; set; }
    public int updatedAt { get; set; }
    public int closedAt { get; set; }
    public long createdBy { get; set; }
    public bool createdByMe { get; set; }
    public int timestamp { get; set; }
    public object image { get; set; }
    public long sender { get; set; }
}

public class RawMeta
{
    public int statusCode { get; set; }
    public int timestamp { get; set; }
}

public class RawInfowijsMessages
{
    public RawData data { get; set; }
    public RawMeta meta { get; set; }
}
#endregion

#region models - Cleaned
public class Message
{
    public string id { get; set; }
    public int type { get; set; }
    public string title { get; set; }
    public string content { get; set; }
    public object createdBy { get; set; }
    public int createdAt { get; set; }
    public bool broadcast { get; set; }
    public object receiver { get; set; }
    public string groupId { get; set; }
    public bool isMe { get; set; }
}

public class Meta
{
    public int statusCode { get; set; }
    public int timestamp { get; set; }
}

public class InfowijsMessages
{
    public List<Message> messages { get; set; }
    public Meta meta { get; set; }
}
#endregion