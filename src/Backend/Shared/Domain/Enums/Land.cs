using System.Text.Json.Serialization;

namespace Shared.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Land
    {
        Deutschland
    }
}
