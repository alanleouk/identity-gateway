using System.Text.Json.Serialization;

namespace Identity.Models
{
    public class PublicJsonWebKey
    {
        [JsonPropertyName("kty")]
        public string? Kty { get; set; }

        [JsonPropertyName("use")]
        public string? Use { get; set; }

        [JsonPropertyName("kid")]
        public string? Kid { get; set; }
    
        [JsonPropertyName("crv")]
        public string? Crv { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("e")]
        public string? E { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("n")]
        public string? N { get; set; }
    
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("x")]
        public string? X { get; set; }
    
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("y")]
        public string? Y { get; set; }

        [JsonPropertyName("alg")]
        public string? Alg { get; set; }
    }
}
