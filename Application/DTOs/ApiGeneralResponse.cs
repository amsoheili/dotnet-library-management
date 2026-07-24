using System.Text.Json.Serialization;

public class ApiGeneralResponse<T>
{
    [JsonPropertyName("result")]
    public T Result { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; } = true;

    [JsonPropertyName("error")]
    public bool error { get; set; }
}