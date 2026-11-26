using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Serverr.Messages;

public class Message
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("data")]
    public Dictionary<string, string> Data { get; set; } = new();

    // הופך את האובייקט למחרוזת JSON (לשליחה)
    public string ToJson() => JsonSerializer.Serialize(this);

    // בונה Message ממחרוזת JSON שהתקבלה
    public static Message FromJson(string json) => JsonSerializer.Deserialize<Message>(json)!;
}