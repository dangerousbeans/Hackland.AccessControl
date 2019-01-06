using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models
{
    public enum MessageType { Success, Error, Info, Warning }
    [Serializable]
    public class MessageModel
    {
        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "redirect")]
        public MessageRedirectModel Redirect { get; set; }
        public static string TempDataKey => "Messages";

        public MessageModel() { }
        public MessageModel(MessageType type, string title, string text)
        {
            this.Type = type;
            this.Title = title;
            this.Text = text;
        }

        public static MessageModel CreateSuccessMessage(string title, string text)
        {
            return new MessageModel(MessageType.Success, title, text);
        }
        public static MessageModel CreateSuccessMessage(string title, string format, params object[] arguments)
        {
            return new MessageModel(MessageType.Success, title, string.Format(format, arguments));
        }
        public static MessageModel CreateErrorMessage(string title, string text)
        {
            return new MessageModel(MessageType.Error, title, text);
        }
        public static MessageModel CreateErrorMessage(string title, string format, params object[] arguments)
        {
            return new MessageModel(MessageType.Error, title, string.Format(format, arguments));
        }
        public static MessageModel CreateInfoMessage(string title, string text)
        {
            return new MessageModel(MessageType.Info, title, text);
        }
        public static MessageModel CreateInfoMessage(string title, string format, params object[] arguments)
        {
            return new MessageModel(MessageType.Info, title, string.Format(format, arguments));
        }
        public static MessageModel CreateWarningMessage(string title, string text, bool clear = true)
        {
            return new MessageModel(MessageType.Warning, title, text);
        }
        public static MessageModel CreateWarningMessage(string title, string format, params object[] arguments)
        {
            return new MessageModel(MessageType.Warning, title, string.Format(format, arguments));
        }
    }

    [Serializable]
    public class MessageRedirectModel
    {
        public string Url { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}
