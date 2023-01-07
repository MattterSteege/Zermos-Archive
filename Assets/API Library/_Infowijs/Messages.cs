using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Messages : MonoBehaviour
{
    [SerializeField] SessionAuthenticatorInfowijs authenticateInfowijs;
    [SerializeField] int maxTryCount = 3;
    
    [ContextMenu("Test")]
    public void Test()
    {
        GetBetterInfowijsMessages();
    }
    
    public RawInfowijsMessages GetInfowijsMessages(bool includeArchived = true, int since = 0)
    {
        
        var access_token = authenticateInfowijs.GetAccesToken(LocalPrefs.GetString("infowijs-access_token"));
        
        if (access_token == null)
        {
            Debug.Log("No access token");
            return null;
        }
        
        var baseurl = string.Format($"https://antonius.hoyapp.nl/hoy/v3/messages?include_archived={(includeArchived ? "1" : "0")}&since={since}");

        var www = UnityWebRequest.Get(baseurl);
        www.SetRequestHeader("authorization", "Bearer " + access_token.data);
        www.SendWebRequest();

        while (!www.isDone)
        {
        }

        var response = JsonConvert.DeserializeObject<RawInfowijsMessages>(www.downloadHandler.text);

        www.Dispose();
        
        while (response.data.hasMore && maxTryCount > 0)
        {
            maxTryCount--;
            var moreMessages = GetInfowijsMessages(includeArchived, response.data.since);
            response.data.messages.AddRange(moreMessages.data.messages);
            response.data.since = moreMessages.data.since;
            response.data.hasMore = moreMessages.data.hasMore;
        }
        maxTryCount = 3;
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
    
    public IEnumerator GetBetterInfowijsMessages()
    {
        var messages = GetInfowijsMessages()?.data.messages.OrderByDescending(x => x.createdAt).ToList() ?? new List<RawMessage>();
        if (messages.Count == 0)
        {
            Debug.Log("No messages");
            yield return null;
        }
        messages.RemoveAll(x => x.type == 2); // TODO: add support for attachments
        messages.RemoveAll(x => x.type == 3); // TODO: add support for foto's
        messages.RemoveAll(x => x.type == 12);
        messages.RemoveAll(x => x.type == 30); // TODO: add support for titles
        
        List<Message> betterMessages = new List<Message>();


        int index = 0;
        foreach (RawMessage message in messages)
        {

            betterMessages.Add(new Message
            {
                id = index.ToString(),
                type = message.type,
                title = "Error getting title",
                content = message.content.ToString(),
                createdAt = message.createdAt,
                broadcast = message.broadcast,
                receiver = message.receiver,
                groupId = message.groupId,
                isMe = message.isMe,
                createdBy = message.createdBy,

            });
            
            index++;
            yield return null;
        }
        
        //i * 2 because we want to skip every other message
        //i * 2 + 1

        yield return betterMessages;
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