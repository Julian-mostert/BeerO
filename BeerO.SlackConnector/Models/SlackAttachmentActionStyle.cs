﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BeerO.SlackConnector.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SlackAttachmentActionStyle
    {
        [EnumMember(Value = "default")]
        Default,
        [EnumMember(Value = "primary")]
        Primary,
        [EnumMember(Value = "danger")]
        Danger
    }
}