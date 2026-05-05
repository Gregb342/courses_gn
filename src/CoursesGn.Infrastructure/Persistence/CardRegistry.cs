using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CoursesGn.Domain.Interfaces;
using CoursesGn.Domain.Models;

namespace CoursesGn.Infrastructure.Persistence;

/// <summary>
/// Gère le compteur global de cartes et le registre (log) des générations.
/// Les données sont persistées dans deux fichiers dans le dossier d'exécution :
///   - cartes_registry.json : source de vérité (compteur + historique)
///   - cartes_registry.csv  : export lisible/Excel, mis à jour à chaque génération
/// </summary>
public class CardRegistry : ICardRegistry
{
    private static readonly string RegistryPath =
        Path.Combine(AppContext.BaseDirectory, "cartes_registry.json");

    private static readonly string CsvPath =
        Path.Combine(AppContext.BaseDirectory, "cartes_registry.csv");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private RegistryData _data;

    public CardRegistry()
    {
        _data = Load();
    }

    // ──────────────────────────────────────────────
    //  Lecture publique
    // ──────────────────────────────────────────────

    public int LastNumber => _data.LastNumber;
    public int TotalGenerated => _data.Entries.Count;

    // ──────────────────────────────────────────────
    //  Obtenir le prochain numéro de carte
    // ──────────────────────────────────────────────

    public int NextCardNumber()
    {
        return _data.LastNumber + 1;
    }

    // ──────────────────────────────────────────────
    //  Enregistrer une carte générée
    // ──────────────────────────────────────────────

    public void RecordCard(CardRegistryEntry entry)
    {
        _data.LastNumber = entry.CardNumber;
        _data.Entries.Add(entry);
        Save();
    }

    // ──────────────────────────────────────────────
    //  Remise à zéro du compteur
    // ──────────────────────────────────────────────

    /// <summary>
    /// Remet le compteur à 0. L'historique est conservé mais marqué d'un séparateur.
    /// </summary>
    public void Reset()
    {
        _data.LastNumber = 0;
        _data.Entries.Add(new CardRegistryEntry
        {
            CardNumber = 0,
            Type = "RESET",
            GeneratedAt = DateTime.Now,
            FileName = $"--- Compteur remis à zéro le {DateTime.Now:dd/MM/yyyy HH:mm} ---"
        });
        Save();
    }

    // ──────────────────────────────────────────────
    //  Persistance JSON + CSV
    // ──────────────────────────────────────────────

    private RegistryData Load()
    {
        if (!File.Exists(RegistryPath))
            return new RegistryData();

        try
        {
            string json = File.ReadAllText(RegistryPath);
            return JsonSerializer.Deserialize<RegistryData>(json, JsonOptions) ?? new RegistryData();
        }
        catch
        {
            return new RegistryData();
        }
    }

    private void Save()
    {
        string json = JsonSerializer.Serialize(_data, JsonOptions);
        File.WriteAllText(RegistryPath, json);
        ExportCsv();
    }

    private void ExportCsv()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Numéro;Type;Difficulté;Flèches;Style;Format;Fichier;Date");

        foreach (var entry in _data.Entries)
        {
            if (entry.Type == "RESET")
            {
                sb.AppendLine($";;;;;;{entry.FileName};");
                continue;
            }

            string difficulty = entry.Difficulty switch
            {
                Domain.Enums.Difficulty.Level1_Easy      => "Facile",
                Domain.Enums.Difficulty.Level2_Normal     => "Normal",
                Domain.Enums.Difficulty.Level3_Medium     => "Moyen",
                Domain.Enums.Difficulty.Level4_Hard       => "Difficile",
                Domain.Enums.Difficulty.Level5_Nightmare  => "Cauchemar",
                _ => entry.Difficulty.ToString()
            };

            sb.AppendLine(
                $"{entry.CardNumber};" +
                $"{entry.Type};" +
                $"{difficulty};" +
                $"{entry.ArrowCount};" +
                $"{entry.ArrowStyle};" +
                $"{entry.OutputFormat};" +
                $"{entry.FileName};" +
                $"{entry.GeneratedAt:dd/MM/yyyy HH:mm:ss}");
        }

        File.WriteAllText(CsvPath, sb.ToString(), Encoding.UTF8);
    }

    // ──────────────────────────────────────────────
    //  Modèle interne JSON
    // ──────────────────────────────────────────────

    private class RegistryData
    {
        public int LastNumber { get; set; } = 0;
        public List<CardRegistryEntry> Entries { get; set; } = [];
    }
}
