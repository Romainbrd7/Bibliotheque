using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static readonly HttpClient client = new HttpClient();
    const string apiUrl = "http://localhost:5062/Livres";

    static async Task Main()
    {
        while (true)
        {
            Console.WriteLine("\n📚 Gestionnaire de Bibliothèque");
            Console.WriteLine("1. Afficher tous les livres");
            Console.WriteLine("2. Rechercher un livre par auteur");
            Console.WriteLine("3. Rechercher un livre par titre");
            Console.WriteLine("4. Ajouter un livre");
            Console.WriteLine("5. Modifier un livre");
            Console.WriteLine("6. Supprimer un livre");

            Console.WriteLine("0. Quitter");
            Console.Write("Choix : ");
            var choix = Console.ReadLine();

            switch (choix)
            {
                case "1": await AfficherTousLesLivres(); break;
                case "2": await RechercherLivreParAuteur(); break;
                case "3": await RechercherParTitre(); break;
                case "4": await AjouterLivre(); break;
                case "5": await ModifierLivre(); break;
                case "6": await SupprimerLivre(); break;
                

                case "0": return;
                default: Console.WriteLine("❌ Choix invalide"); break;
            }
        }
    }

    static async Task AfficherTousLesLivres()
    {
        try
        {
            var livres = await client.GetFromJsonAsync<dynamic[]>(apiUrl);
            foreach (var livre in livres)
            {
                Console.WriteLine(JsonSerializer.Serialize(livre, new JsonSerializerOptions { WriteIndented = true }));
            }
        }
        catch
        {
            Console.WriteLine("❌ Erreur lors de la récupération des livres.");
        }
    }

static async Task RechercherLivreParAuteur()
{
    Console.Write("Auteur à rechercher : ");
    var auteur = Console.ReadLine();

    var url = $"{apiUrl}?author={Uri.EscapeDataString(auteur ?? "")}";

    try
    {
        var livres = await client.GetFromJsonAsync<dynamic[]>(url);
        if (livres == null || livres.Length == 0)
        {
            Console.WriteLine("❌ Aucun résultat trouvé.");
            return;
        }

        foreach (var livre in livres)
        {
            Console.WriteLine(JsonSerializer.Serialize(livre, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erreur lors de la recherche : {ex.Message}");
    }
}

static async Task RechercherParTitre()
{
    Console.Write("Titre à rechercher : ");
    var titre = Console.ReadLine();

    var url = $"{apiUrl}?title={Uri.EscapeDataString(titre ?? "")}";

    try
    {
        var livres = await client.GetFromJsonAsync<dynamic[]>(url);
        if (livres == null || livres.Length == 0)
        {
            Console.WriteLine("❌ Aucun résultat trouvé.");
            return;
        }

        foreach (var livre in livres)
        {
            Console.WriteLine(JsonSerializer.Serialize(livre, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erreur lors de la recherche : {ex.Message}");
    }
}

    static async Task AjouterLivre()
{
    Console.Write("Titre : ");
    string titre = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Auteur : ");
    string auteur = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Année de publication : ");
    if (!int.TryParse(Console.ReadLine(), out int annee) || annee <= 0)
    {
        Console.WriteLine("❌ Année invalide.");
        return;
    }

    Console.Write("Type (ebook ou paperbook) : ");
    string type = Console.ReadLine()?.Trim().ToLower();

    object? nouveauLivre = type switch
    {
        "ebook" => new
        {
            title = titre,
            author = auteur,
            year = annee,
            type = "ebook",
            format = SaisirFormat()
        },
        "paperbook" => new
        {
            title = titre,
            author = auteur,
            year = annee,
            type = "paperbook",
            pageCount = SaisirPageCount()
        },
        _ => null
    };

    if (string.IsNullOrWhiteSpace(titre) || string.IsNullOrWhiteSpace(auteur) || nouveauLivre == null)
    {
        Console.WriteLine("❌ Données invalides. Le titre, l’auteur et le type doivent être remplis.");
        return;
    }

    var response = await client.PostAsJsonAsync(apiUrl, nouveauLivre);

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("✅ Livre ajouté avec succès !");
    }
    else
    {
        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"❌ Échec de l’ajout : {response.StatusCode}\n{error}");
    }
}


   static async Task ModifierLivre()
{
    Console.Write("ID du livre à modifier : ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("❌ ID invalide.");
        return;
    }

    Console.Write("Nouveau titre : ");
    string titre = Console.ReadLine();

    Console.Write("Nouvel auteur : ");
    string auteur = Console.ReadLine();

    Console.Write("Nouvelle année : ");
    if (!int.TryParse(Console.ReadLine(), out int annee))
    {
        Console.WriteLine("❌ Année invalide.");
        return;
    }

    Console.Write("Type (ebook ou paperbook) : ");
    string type = Console.ReadLine().Trim().ToLower();

    object? livreModifie = type switch
    {
        "ebook" => new
        {
            id,
            title = titre,
            author = auteur,
            year = annee,
            type = "ebook",
            format = SaisirFormat()
        },
        "paperbook" => new
        {
            id,
            title = titre,
            author = auteur,
            year = annee,
            type = "paperbook",
            pageCount = SaisirPageCount()
        },
        _ => null
    };

    if (livreModifie == null)
    {
        Console.WriteLine("❌ Type inconnu.");
        return;
    }

    var response = await client.PutAsJsonAsync($"{apiUrl}/{id}", livreModifie);
if (response.IsSuccessStatusCode)
{
    Console.WriteLine("✅ Livre modifié.");
}
else
{
    var erreur = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"❌ Erreur : {(int)response.StatusCode} - {erreur}");
}

}

// Demande le format pour ebook
static string SaisirFormat()
{
    Console.Write("Format (PDF, EPUB...) : ");
    return Console.ReadLine() ?? "PDF";
}

// Demande le nombre de pages pour paperbook
static int SaisirPageCount()
{
    Console.Write("Nombre de pages : ");
    int.TryParse(Console.ReadLine(), out int pages);
    return pages > 0 ? pages : 100;
}

    static async Task SupprimerLivre()
    {
        Console.Write("ID du livre à supprimer : ");
        int id = int.Parse(Console.ReadLine()!);
        var response = await client.DeleteAsync($"{apiUrl}/{id}");
        Console.WriteLine(response.IsSuccessStatusCode ? "✅ Livre supprimé." : "❌ Échec de la suppression.");
    }
}
