using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models;

public class Ebook : Media, IReadable
{
    [Required]
    [JsonPropertyName("format")]
    public string Format { get; set; } = "PDF";

    public override string DisplayInformation()
    {
        return $"{Title} by {Author} (eBook - Format: {Format})";
    }
}
