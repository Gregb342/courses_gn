using CoursesGn.Domain.Enums;
using CoursesGn.Domain.Interfaces;
using CoursesGn.Domain.Models;

namespace CoursesGn.Application.Services;

/// <summary>
/// Orchestrateur principal : coordonne la génération, le rendu et l'export des cartes.
/// </summary>
public class NavigationCardOrchestrator
{
    private readonly ICourseGenerator _courseGenerator;
    private readonly ICardRenderer _cardRenderer;
    private readonly IFileExporter _fileExporter;

    public NavigationCardOrchestrator(
        ICourseGenerator courseGenerator,
        ICardRenderer cardRenderer,
        IFileExporter fileExporter)
    {
        _courseGenerator = courseGenerator;
        _cardRenderer = cardRenderer;
        _fileExporter = fileExporter;
    }

    /// <summary>
    /// Génère toutes les cartes demandées et les exporte dans le répertoire spécifié.
    /// Pour chaque parcours, génère une carte PJ (au niveau de difficulté choisi)
    /// et une carte PNJ (toujours en mode texte facile).
    /// Retourne la liste des chemins de fichiers créés.
    /// </summary>
    public List<string> GenerateCards(GenerationParameters parameters, string outputDirectory)
    {
        var generatedFiles = new List<string>();

        // Paramètres identiques mais forcés en Level1_Easy pour la carte PNJ
        var pnjParameters = new GenerationParameters
        {
            ArrowCount = parameters.ArrowCount,
            CardCount = parameters.CardCount,
            OutputFormat = parameters.OutputFormat,
            ArrowStyle = parameters.ArrowStyle,
            Difficulty = Difficulty.Level1_Easy
        };

        for (int i = 1; i <= parameters.CardCount; i++)
        {
            Console.WriteLine($"  Génération de la carte {i}/{parameters.CardCount}...");

            var course = _courseGenerator.Generate(parameters);

            // ── Carte PJ (difficulté choisie) ──
            byte[] pjData = _cardRenderer.Render(course, parameters, "Carte PJ");

            string pjPath = _fileExporter.Export(
                pjData,
                parameters.OutputFormat,
                parameters.Difficulty,
                i,
                outputDirectory,
                isPnj: false);

            generatedFiles.Add(pjPath);
            Console.WriteLine($"    ✓ {Path.GetFileName(pjPath)}");

            // ── Carte PNJ (toujours en mode texte facile) ──
            byte[] pnjData = _cardRenderer.Render(course, pnjParameters, "Carte PNJ");

            string pnjPath = _fileExporter.Export(
                pnjData,
                parameters.OutputFormat,
                parameters.Difficulty,
                i,
                outputDirectory,
                isPnj: true);

            generatedFiles.Add(pnjPath);
            Console.WriteLine($"    ✓ {Path.GetFileName(pnjPath)}");
        }

        return generatedFiles;
    }
}
