using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models;

public class PaperBook : Media, IReadable
{
    [Range(1, 5000)]
    [JsonPropertyName("pageCount")]
    public int PageCount { get; set; }

    public override string DisplayInformation()
    {
        return $"{Title} by {Author} (Paper book - {PageCount} pages)";
    }
}
