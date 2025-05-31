using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Repositories;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class LivresController : ControllerBase
{
    private readonly Repository<Media> _repository;

    public LivresController()
    {
        _repository = new Repository<Media>(); // Simule un "injectable" en mémoire
    }

    [HttpGet]
    public ActionResult<IEnumerable<Media>> GetAll([FromQuery] string? author, [FromQuery] string? title, [FromQuery] string? sort)
    {
        var items = _repository.GetAll();

        if (!string.IsNullOrEmpty(author))
            items = items.Where(x => x.Author.Contains(author, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(title))
            items = items.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(sort))
        {
            items = sort.ToLower() switch
            {
                "author" => items.OrderBy(x => x.Author),
                "title" => items.OrderBy(x => x.Title),
                _ => items
            };
        }

        return Ok(items);
    }

    [HttpGet("{id}")]
    public ActionResult<Media> Get(int id)
    {
        var item = _repository.Get(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public IActionResult Post([FromBody] JsonElement json)
    {
        if (!json.TryGetProperty("type", out var typeProp))
        {
            return BadRequest("Le champ 'type' est requis (ebook ou paperbook).");
        }

        var type = typeProp.GetString();
        var jsonRaw = json.GetRawText();

        Media? media = type?.ToLower() switch
        {
            "ebook" => JsonSerializer.Deserialize<Ebook>(jsonRaw),
            "paperbook" => JsonSerializer.Deserialize<PaperBook>(jsonRaw),
            _ => null
        };

        if (media == null)
        {
            return BadRequest("Type non reconnu ou désérialisation échouée.");
        }

        // 🔍 VALIDATION MANUELLE
        var validationContext = new ValidationContext(media);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(media, validationContext, results, true))
        {
            return BadRequest(results);
        }

        _repository.Add(media);
        return CreatedAtAction(nameof(Get), new { id = media.Id }, media);
    }
}
