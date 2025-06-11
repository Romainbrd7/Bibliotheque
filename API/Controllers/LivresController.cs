using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Repositories;
using System.Text.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LivresController : ControllerBase
    {
        private readonly IMediaRepository _repository;

        public LivresController(IMediaRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Media>>> GetAll([FromQuery] string? author, [FromQuery] string? title)
        {
            var livres = await _repository.GetAllAsync(author, title);
            return Ok(livres);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Media>> Get(int id)
        {
            var livre = await _repository.GetByIdAsync(id);
            if (livre == null)
                return NotFound();
            return Ok(livre);
        }

        [HttpPost]
public async Task<ActionResult<Media>> Create([FromBody] JsonElement json)
{
    if (!json.TryGetProperty("type", out var typeProp))
        return BadRequest("Le champ 'type' est requis (ebook ou paperbook)");

    var type = typeProp.GetString();
    Media media = type switch
    {
        "ebook" => JsonSerializer.Deserialize<Ebook>(json.ToString())!,
        "paperbook" => JsonSerializer.Deserialize<PaperBook>(json.ToString())!,
        _ => throw new ArgumentException("Type de média inconnu")
    };

    // 🔍 Validation manuelle
    if (string.IsNullOrWhiteSpace(media.Title))
    return BadRequest("❌ Titre manquant ou vide.");
if (string.IsNullOrWhiteSpace(media.Author))
    return BadRequest("❌ Auteur manquant ou vide.");
if (media.Year < 1800 || media.Year > 2100)
    return BadRequest("❌ Année invalide.");


    await _repository.AddAsync(media);
    return CreatedAtAction(nameof(Get), new { id = media.Id }, media);
}


        [HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromBody] JsonElement json)
{
    try
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        if (!json.TryGetProperty("type", out var typeProp))
            return BadRequest("Le champ 'type' est requis pour la mise à jour");

        var type = typeProp.GetString();
        Media updated = type switch
        {
            "ebook" => JsonSerializer.Deserialize<Ebook>(json.ToString())!,
            "paperbook" => JsonSerializer.Deserialize<PaperBook>(json.ToString())!,
            _ => throw new ArgumentException("Type de média inconnu")
        };

        updated.Id = id;
        await _repository.UpdateAsync(updated);
        return NoContent();
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Erreur côté serveur : {ex.Message}");
    }
}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
