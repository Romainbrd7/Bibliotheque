using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public abstract class Media : IReadable
{
    public int Id { get; set; }

    [Required]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    [Range(1800, 2100)]
    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("type")]
    public string Type => GetType().Name.ToLower(); // "ebook" ou "paperbook"

    public abstract string DisplayInformation();
}

}
